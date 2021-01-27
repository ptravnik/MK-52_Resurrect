//////////////////////////////////////////////////////////
//
//  MK-52 RESURRECT
//  Copyright (c) 2020 Mike Yakimov.  All rights reserved.
//  See main file for the license
//
//////////////////////////////////////////////////////////

#ifndef RPN_FUNCTIONS_HPP
#define RPN_FUNCTIONS_HPP

#include "SD_Manager.hpp"
#include "Program_Memory.hpp"
#include "Register_Memory.hpp"

namespace MK52_Interpreter{

    class RPN_Function{
        public:
            virtual bool checkID( uint16_t id);
            virtual bool checkName(char *name);
            virtual const char*Name();
            virtual const char*IOName();
            virtual void execute(void *components[], char *command=NULL);
        protected:
            RPN_Stack *_dealWithClergy1(void *components[]);
            RPN_Stack *_dealWithClergy2(void *components[]);
            bool _startsWith(char *name, const char *keyword);
    };

    class RPN_Functions{
        public:
            bool _atStop = false;
            RPN_Stack *Stack;
            Program_Memory *progMem;
            Extended_Memory *extMem;

            unsigned long init( void *components[]);
            RPN_Function *getFunctionByID(int16_t id);
            RPN_Function *getFunctionByName(char *command);
            void execute( int16_t id=-1, char *command=NULL);
            void execute( char *command, bool pushNeeded=false);
            void executeStep();

      private:
            void **_components;
            void _appendFunction( RPN_Function *f);
            uint16_t _nfunctions = 0;
            void *_functions[MK52_NFUNCTIONS];
    };
};

#endif // RPN_FUNCTIONS_HPP
