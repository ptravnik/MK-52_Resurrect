//////////////////////////////////////////////////////////
//
//  MK-52 RESURRECT
//  Copyright (c) 2020 Mike Yakimov.  All rights reserved.
//  See main file for the license
//
//////////////////////////////////////////////////////////

#ifndef PROGRAM_MEMORY_HPP
#define PROGRAM_MEMORY_HPP

#include "UniversalValue.hpp"

namespace MK52_Interpreter{

    class Program_Memory{
      public:
        void init( void *components[]);
        void clear();
        void resetCounter();

        inline uint32_t getCounter(){ return _counter;};
        uint32_t setCounter(uint32_t address);
        uint32_t setCounter(char *text);
        uint32_t setCounterToBottom();
        bool incrementCounter();
        bool decrementCounter();
        bool goSub( uint32_t address);
        bool goSub( char *text);
        bool returnFromSub();

        inline uint32_t getFree(){return (uint32_t)(PROGRAM_MEMORY_SIZE-_bottom);};
        inline char *getCurrentLine(){return (char*)(_buffer+_current);};
        inline char currentChar(){return (char)_buffer[_current];};
        inline char *getBottom(){return (char*)(_buffer+_bottom);};
        inline bool isAtEnd(){return _current >= _bottom;};

        bool appendLine(char *line);
        bool appendLine_P(const char *line);
        bool replaceLine(char *line);
        bool replaceLine_P(const char *line);
        bool insertLine(char *line);
        bool insertLine_P(const char *line);
        bool updateLine(char *line);
        bool updateLine_P(const char *line);
        void deleteLine();
        void renumberAdresses( uint32_t fromLine, int32_t shift);
        bool commentLine();

        inline uint8_t getEMode(){ return _eMode;}; 
        inline char *getEModeName(){ return _eModeName;}; 
        void setEMode(uint8_t m); 
        uint8_t toggleEditMode();

        void getPreviousLines( char *lines[], uint8_t n);
        bool isAtStop();

        char *toString( char *text, int16_t textLength);
        char *toCounterString( char *text, int16_t textLength);

        inline uint32_t getCallStackPtr(){
          return _returnStackPtr >> 1;};
        char *getCallStackValues( char *text, uint16_t index);
        void setCallStackValues( uint16_t index, char *ctr, char *ptr);

      private:
        void **_components;
        uint32_t _counter = 0;
        uint32_t _bottom = 1;
        uint32_t _current = 0;
        uint8_t *_buffer = NULL;
        uint8_t _eMode = EMODE_OWERWRITE;
        char _eModeName[4];
        uint32_t *_returnStack = NULL;
        uint32_t _returnStackPtr = 0;
        char *_modifyAddress(char *address, uint32_t fromLine, int32_t shift);
        bool _moveStringsFromCurrent(int32_t shift);
    };
};

#endif // PROGRAM_MEMORY_HPP
