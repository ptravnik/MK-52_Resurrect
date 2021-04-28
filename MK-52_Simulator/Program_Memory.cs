//////////////////////////////////////////////////////////
//
//  MK-52 RESURRECT
//  Copyright (c) 2020 Mike Yakimov.  All rights reserved.
//  See main file for the license
//
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MK52Simulator
{
    /// <summary>
    /// Implements program storage (as prototype)
    /// In the original MK-52, there are 103 slots of program memory 
    /// In the Resurrect, there is a space of 64000 bytes, the actual
    /// number of program steps is determined by the command length.
    /// </summary>
    public class Program_Memory
    {
        public const int EMODE_OWERWRITE = 0;
        public const int EMODE_INSERT = 1;
        public const int EMODE_SHIFT = 2;
        public const int PROGRAM_MEMORY_SIZE = 64000;
        public const int RETURN_STACK_SIZE = 100;

        private uint _counter = 0;
        private uint _bottom = 1;
        private uint _current = 0;
        private byte[] _buffer = new byte[PROGRAM_MEMORY_SIZE];
        private MK52_Host _parent = null;

        private int _eMode = EMODE_OWERWRITE;
        private string[] _emodeLabels = { "OVR", "INS", "SHF" };

        private uint[] _returnStack_ptrs = new uint[RETURN_STACK_SIZE];
        private uint[] _returnStack_ctrs = new uint[RETURN_STACK_SIZE];
        private int _returnStackPtr = 0;

        public void init( MK52_Host components)
        {
            _parent = components;
            clear();
            setEMode(EMODE_OWERWRITE);
        }

        public void clear()
        {
            for( int i=0; i<_buffer.Length; i++)
                _buffer[i] = 0;
            _bottom = 1; // one empty line
            resetCounter();
        }

        public void resetCounter()
        {
            _current = 0;
            _counter = 0;
            _returnStackPtr = 0;
        }

        public uint getCounter()
        {
            return _counter;
        }

        public uint setCounter(uint address)
        {
            while (_counter < address)
            {
                if (incrementCounter()) break;
            }
            while (_counter > address)
            {
                if (decrementCounter()) break;
            }
            return _counter;
        }

        public uint setCounter(string text)
        {
            int ln = text.Length;
            if (ln <= 0) return _counter;
            if (text[0] == ' ') return _counter;
            return setCounter(Convert.ToUInt32(text.Trim()));
        }

        public uint setCounterToBottom()
        {
            while (_current < _bottom)
            {
                if (incrementCounter()) break;
            }
            while (_current > _bottom)
            {
                if (decrementCounter()) break;
            }
            return _counter;
        }

        public bool incrementCounter()
        {
            // Skip to end of line
            uint tmp = _current;
            while (_current < PROGRAM_MEMORY_SIZE && currentChar() != 0) _current++;

            // End of memory reached - nothing changes
            if (_current >= PROGRAM_MEMORY_SIZE)
            {
                _current = tmp;
                return true;
            }
            _current++; // next line
            _counter++;
            return false;
        }

        public bool decrementCounter()
        {
            if (_current == 0) return true; // already at top

            // Skip to end of previous line
            int tmp = (int)_current;
            tmp--;
            if (tmp <= 0)
            {
                _counter = 0;
                _current = 0;
                return false;
            }

            // Skip to beginning of previous line
            tmp--;
            while (tmp >= 0 && _buffer[tmp] != 0) tmp--;
            if (tmp < 0)
            {
                _counter = 0;
                _current = 0;
                return false;
            }
            _current = Convert.ToUInt32( tmp + 1);
            _counter--;
            return false;
        }

        public bool goSub( uint address)
        {
            if (_returnStackPtr >= RETURN_STACK_SIZE) return true;
            _returnStack_ptrs[_returnStackPtr] = _current;
            _returnStack_ctrs[_returnStackPtr] = _counter;
            _returnStackPtr++;
            setCounter(address);
            return false;
        }

        public bool goSub( string text)
        {
            if (_returnStackPtr >= RETURN_STACK_SIZE) return true;
            _returnStack_ptrs[_returnStackPtr] = _current;
            _returnStack_ctrs[_returnStackPtr] = _counter;
            _returnStackPtr++;
            setCounter(text);
            return false;
        }

        public bool returnFromSub()
        {
            if (_returnStackPtr == 0)
            {
                _current = 0;
                _counter = 0;
            }
            else
            {
                _returnStackPtr--;
                _current = _returnStack_ptrs[_returnStackPtr];
                _counter = _returnStack_ctrs[_returnStackPtr];
            }
            incrementCounter(); // go to the next line after call (mimic actual MK-52)
            return false;
        }

        public uint getFree()
        {
            return (uint)(PROGRAM_MEMORY_SIZE-_bottom);
        }

        public string getCurrentLine()
        {
            Encoding _m_Enc = Encoding.GetEncoding(1251);
            StringBuilder sb = new StringBuilder();
            int j = 0;
            for( uint i=_current; _buffer[i]!=0; i++) j++;
            char[] foo = _m_Enc.GetChars(_buffer, (int)_current, j);
            sb.Append( foo);
            string bar = sb.ToString();
            return bar;
        }

        public char currentChar()
        {
            return (char)_buffer[_current];
        }

        public bool isAtEnd()
        {
            return _current >= _bottom;
        }

        public bool appendLine(string line)
        {
            return appendLine_P( line);
        }

        public bool appendLine_P(string line)
        {
            byte[] tmp = Encoding.GetEncoding(1251).GetBytes(line);
            int toAppend = tmp.Length+1;
            if (_current + toAppend >= PROGRAM_MEMORY_SIZE) return true; // no space
            _bottom = _current;
            toAppend--;
            for (int i = 0; i < toAppend; i++, _bottom++)
                _buffer[_bottom] = tmp[i];
            _buffer[_bottom] = 0;
            return false;
        }

        public bool replaceLine(string line)
        {
            return replaceLine_P( line);
        }

        private bool __copyStringToCurrent(string line)
        {
            byte [] tmp = Encoding.GetEncoding(1251).GetBytes(line);
            uint j = _current;
            for (int i = 0; i < tmp.Length; i++, j++)
                _buffer[j] = tmp[i];
            _buffer[j] = 0;
            return false;
        }

        private bool __moveStringsFromCurrent(int shift)
        {
            if (shift > 0)
            {
                if (_bottom + (uint)shift >= PROGRAM_MEMORY_SIZE) return true;
                for (int i = (int)_bottom; i >= _current; i--)
                    _buffer[i + shift] = _buffer[i];
                _bottom += (uint)shift;
            }
            if (shift < 0)
            {
                for (int i = (int)_current-shift; i <= _bottom; i++)
                    _buffer[i+shift] = _buffer[i];
                for (int i = shift; i < 0; i++, _bottom--)
                    _buffer[_bottom] = 0;
            }
            return false;
        }

        public bool replaceLine_P(string line)
        {
            if (_current >= _bottom) return appendLine_P(line);
            int toCopy = line.Length + 1;
            string ptrC = getCurrentLine();
            int toReplace = ptrC.Length + 1;
            if( __moveStringsFromCurrent(toCopy - toReplace)) return true;
            return __copyStringToCurrent(line);
        }

        public bool insertLine(string line)
        {
            return insertLine_P(line);
        }

        public bool insertLine_P( string line)
        {
            if (_current >= _bottom) return appendLine_P(line);
            int toInsert = line.Length + 1;
            if (__moveStringsFromCurrent(toInsert)) return true;
            return __copyStringToCurrent(line);
        }

        public bool updateLine(string line)
        {
            return updateLine_P( line);
        }

        public bool updateLine_P( string line)
        {
            bool result;
            switch (_eMode)
            {
                case EMODE_OWERWRITE:
                    return replaceLine_P(line);
                case EMODE_INSERT:
                    return insertLine_P(line);
                default:
                    result = insertLine_P(line);
                    if (!result) renumberAdresses(_counter, 1);
                    break;
            }
            return result;
        }

        public void deleteLine()
        {
            if (_current >= _bottom) return;
            string s = getCurrentLine();
            switch (_eMode)
            {
                case EMODE_OWERWRITE:
                    __moveStringsFromCurrent(-s.Length);
                    break;
                case EMODE_INSERT:
                    __moveStringsFromCurrent(-s.Length-1);
                    break;
                default:
                    __moveStringsFromCurrent(-s.Length-1);
                    renumberAdresses(_current, -1);
                    break;
            }
        }

        private string modifyAddress(string address, uint fromLine, int shift)
        {
            if (address.Length <= 0) return address;
            try
            {
                int addr = Convert.ToInt32(address);
                if (addr <= fromLine) return address;
                addr += shift;
                if (addr < 0) addr = 0;
                return addr.ToString("0000");
            }
            catch
            {
                return address;
            }
        }

        public void renumberAdresses( uint fromLine, int shift)
        {
            uint tmpCounter = _counter;
            resetCounter();
            while (_current < _bottom)
            {
                string progLine = getCurrentLine();
                RPN_Function fn = _parent._m_RPN_Functions.getFunctionByName(progLine);
                if (fn != null && fn.containsPC())
                {
                    int operand = fn.Name().Length;
                    string addrStr = progLine.Substring(operand);
                    addrStr = modifyAddress(addrStr, fromLine, shift);
                    replaceLine(fn.Name() + addrStr);
                }
                incrementCounter();
            }
            setCounter(tmpCounter);
        }

        public bool commentLine()
        {
            if( _current >= _bottom)
                return appendLine_P("#");
            string ptrC = getCurrentLine();
            if( ptrC.StartsWith("#")) return __moveStringsFromCurrent(-1);
            if( __moveStringsFromCurrent( 1 )) return true;
            _buffer[_current] = Convert.ToByte('#');
            return false;
        }

        public int getEMode()
        {
            return _eMode;
        }

        public string getEModeName()
        {
            return _emodeLabels[_eMode];
        }

        public void setEMode(int m)
        {
            if (m > EMODE_SHIFT) m = EMODE_OWERWRITE;
            _eMode = m;
        }

        public int toggleEditMode()
        {
            setEMode(_eMode + 1);
            return _eMode;
        } 
 
        public string[] getPreviousLines( int n)
        {
            List<string> tmp = new List<string>();
            tmp.Add(getCurrentLine());
            if( _counter == 0) return tmp.ToArray();
            uint tmpCtr = _counter;
            while( tmp.Count<n)
            {
                decrementCounter();
                tmp.Add(getCurrentLine());
                if (_counter == 0) break;
            }
            setCounter(tmpCtr);
            return tmp.ToArray();
        }

        public bool isAtStop()
        {
            return UniversalValue._identicalTo_P( getCurrentLine(), "STOP");
        }

        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_counter.ToString("0000"));
            sb.Append("> ");
            sb.Append(getCurrentLine());
            return sb.ToString();
        }

        public string toCounterString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_counter.ToString("0000"));
            sb.Append("> ");
            return sb.ToString();
        }

        public override string ToString()
        {
            return toString();
        }

        public uint getCallStackPtr()
        {
            return (uint)_returnStackPtr;
        }

        public string getCallStackValues( uint n)
        {
            if( n>=_returnStackPtr) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append( "S");
            sb.Append( _returnStack_ctrs[n].ToString("0000"));
            sb.Append( ": ");
            sb.Append( _returnStack_ptrs[n].ToString("0"));
            return sb.ToString();
        }

        public void setCallStackValues( uint n, string ctr, string ptr)
        {
            if( n>= RETURN_STACK_SIZE) return;
            try
            {
                _returnStack_ctrs[n] = Convert.ToUInt32(ctr.Trim());
                _returnStack_ptrs[n] = Convert.ToUInt32(ptr.Trim());
                _returnStackPtr = (int)n + 1;
            }
            catch { }
        }
    }
}
