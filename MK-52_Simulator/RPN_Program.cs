using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MK52Simulator.Functions;
using MK52Simulator.Receivers;

namespace MK52Simulator
{
    //
    // Implements program storage (as prototype)
    // The actual code on C++ will be without Dictionary
    //
    public class RPN_Program
    {
        public const int ProgramSize = 1000;

        private MK52_Host _parent = null;
        private Dictionary<int, string> ProgramMemory = new Dictionary<int, string>();

        public RPN_Counter Counter = new RPN_Counter( "PC", ProgramSize);
        public InputReceiver_String Text = null;
        public InputReceiver_Value Number = null;

        public RPN_Program( MK52_Host parent)
        {
            _parent = parent;
            Text = new InputReceiver_String(parent);
            Number = new InputReceiver_Value( parent);
        }

        public void Clear()
        {
            ProgramMemory.Clear();
            Counter.Set(0);
        }

        public bool isAtStop
        {
            get
            {
                return GetCurrentLine().Trim().StartsWith("STOP");
            }
        }
        
        public string GetCurrentLine()
        {
            return GetLine( Counter.V);
        }

        public void SetCurrentLine( string line)
        {
            SetLine(Counter.V, line);
        }

        public void AppendCurrentLine(string line)
        {
            string tmp = GetCurrentLine();
            SetCurrentLine( tmp + line);
        }

        public void AppendCounterString()
        {
            string tmp = GetCurrentLine();
            if( tmp.Length <= 0) return;
            SetCurrentLine(tmp + Counter.entryResult.ToString("000"));
        }

        public void ExecuteCurrentLine()
        {
            string line = GetCurrentLine().Trim();
            if (_parent.Functions["Empty"].executeCODE(line)) return;
            if (_parent.Functions["Comment"].executeCODE(line)) return;
            if (_parent.Functions["STOP"].executeCODE(line)) return;
            if (_parent.Functions.ContainsKey(line))
            {
                RPN_Function f = _parent.Functions[line];
                f.execute(line);
                Counter.Increment();
                return;
            }
            foreach (string k in _parent.Functions.Keys)
            {
                if( _parent.Functions[k].executeCODE( line))
                    return;
            }
            _parent.CalcStack.X_Label = "Not Found: " + line;
            Counter.Increment();
        }

        public string GetLine( int number)
        {
            if( !ProgramMemory.ContainsKey( number)) return ""; // string empty
            return ProgramMemory[number];
        }

        public void SetLine( int number, string line)
        {
            if (line.Length == 0)
            {
                if( ProgramMemory.ContainsKey(number))
                    ProgramMemory.Remove(number);
                return;
            }
            if (!ProgramMemory.ContainsKey(number))
            {
                ProgramMemory.Add(number, line);
                return;
            }
            ProgramMemory[number] = line;
        }

        public void InsertLine(int number, string line)
        {
            //for( i

            if (line.Length == 0)
            {
                if (ProgramMemory.ContainsKey(number))
                    ProgramMemory.Remove(number);
                return;
            }
            if (!ProgramMemory.ContainsKey(number))
            {
                ProgramMemory.Add(number, line);
                return;
            }
            ProgramMemory[number] = line;
        }

        public bool LoadLine(string s)
        {
            if (!s.StartsWith("P")) return false;
            if (PCLoadHelper(s)) return true;
            int number = Convert.ToInt32(s.Substring(1, 3));
            SetLine( number, s.Substring(5).Trim());
            return true;
        }

        public bool Load(StreamReader sr)
        {
            ProgramMemory.Clear();
            while( !sr.EndOfStream)
            {
                string s = sr.ReadLine().Trim();
                if( s.Length == 0 || s.StartsWith("#")) continue;
                LoadLine(s);
            }
            return true;
        }

        private bool PCLoadHelper(string s)
        {
            if (!s.StartsWith("PC = ")) return false;
            Counter.Set(Convert.ToInt32(s.Substring(5).Trim()));
            return true;
        }

        public void Save(StreamWriter sw)
        {
            sw.Write("#\n");
            sw.Write("# Program:\n");
            sw.Write("#\n");
            sw.Write("PC = " + Counter.V.ToString() + "\n");
            for( int k=0; k<Counter.MaxValue; k++)
            {
                if (!ProgramMemory.ContainsKey(k)) continue;
                string v = ProgramMemory[k];
                if (v.Length == 0) continue;
                sw.Write(k.ToString("P000: ") + v + "\n");
            }
        }
    }
}
