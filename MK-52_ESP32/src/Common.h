//////////////////////////////////////////////////////////
//
//  MK-52 RESURRECT
//  Copyright (c) 2020 Mike Yakimov.  All rights reserved.
//  See main file for the license
//
//////////////////////////////////////////////////////////

#include <Arduino.h>

//
// Serial port for debugging
// 
#define SERIAL_HARD_BAUD_RATE 115200

// Screen dimensions in symbols
// Columns 320 / 11 = 29
// Rows 240 / 20 = 12 (but the top one is taken by the status bar)
#define SCREEN_COLS                 30 
#define CALC_COLS                   20 
#define SCREEN_ROWS                 12
#define PROGRAM_LINE_LENGTH         64 
#define SCREEN_BUFFER_SIZE          372 

// How long the system waits between the keyboard scans
#define KBD_IDLE_DELAY              30
#define DEBUG_SHOW_DELAY            10000

#define VALUE_TYPE_EMPTY            0
#define VALUE_TYPE_INTEGER          1
#define VALUE_TYPE_DOUBLE           2

// not accounting for the Bx (the total size is 5)
#define RPN_STACK_SIZE              4

#define HUGE_POSITIVE_INTEGER       ( 9000000000000000000L)
#define HUGE_NEGATIVE_INTEGER       (-9000000000000000000L)
#define HUGE_POSITIVE_AS_REAL       (9e18)
#define HUGE_NEGATIVE_AS_REAL       (-9e18)

#define DMODE_DEGREES               0
#define DMODE_RADIANS               1
#define DMODE_GRADS                 2

#define PROGRAM_MEMORY_SIZE         64000
#define EXTENDED_MEMORY_SIZE        36000
#define MK52_NFUNCTIONS             100

#define NO_CHANGE                   -1
#define SHUTDOWN_REQUESTED          -2

#define COMPONENT_LCD_MANAGER       0
#define COMPONENT_KBD_MANAGER       1
#define COMPONENT_SD_MANAGER        2

#define COMPONENT_PROGRAM_MEMORY    3
#define COMPONENT_REGISTER_MEMORY   4
#define COMPONENT_EXTENDED_MEMORY   5
#define COMPONENT_STACK             6
#define COMPONENT_FUNCTIONS         7

#define COMPONENT_RECEIVER_NUMBER   8
#define COMPONENT_RECEIVER_ADDRESS  9
#define COMPONENT_RECEIVER_REGISTER 10
#define COMPONENT_RECEIVER_STRING   11

#define COMPONENT_RECEIVER_AUTO_N   12
#define COMPONENT_RECEIVER_AUTO_F   13
#define COMPONENT_RECEIVER_AUTO_K   14
#define COMPONENT_RECEIVER_AUTO_A   15
#define COMPONENT_RECEIVER_AUTO_R   16

#define COMPONENT_RECEIVER_PROG_N   17
#define COMPONENT_RECEIVER_PROG_F   18
#define COMPONENT_RECEIVER_PROG_K   19
#define COMPONENT_RECEIVER_PROG_A   20

#define COMPONENT_RECEIVER_DATA_N   21
#define COMPONENT_RECEIVER_DATA_F   22
#define COMPONENT_RECEIVER_DATA_K   23
#define COMPONENT_RECEIVER_DATA_A   24

#define COMPONENT_RECEIVER_FILE_N   25
#define COMPONENT_RECEIVER_FILE_F   26
#define COMPONENT_RECEIVER_FILE_K   27
#define COMPONENT_RECEIVER_FILE_A   28

#define COMPONENT_DISPLAY_AUTO      29
#define COMPONENT_DISPLAY_PROG      30
#define COMPONENT_DISPLAY_DATA      31
#define COMPONENT_DISPLAY_FILE      32

#define N_COMPONENTS                33
