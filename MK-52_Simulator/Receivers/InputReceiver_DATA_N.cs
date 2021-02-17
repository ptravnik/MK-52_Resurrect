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
    //
    // Implements a generic empty receiver
    //
    public class InputReceiver_DATA_N: Receiver
    {
        public InputReceiver_DATA_N(MK52_Host parent)
            : base( parent)
        {
            Moniker = "LIST_N";
        }

        //public override string tick(MK52_Button button)
        //{
        //    switch (button.Moniker)
        //    {
        //        // Column 0
        //        case "Func F":
        //            _parent.setReceiver("LIST_F");
        //            return "Nothing";
        //        case "Func K":
        //            _parent.setReceiver("LIST_K");
        //            return "Nothing";
        //        case "Func A":
        //            _parent.setReceiver("LIST_A");
        //            return "Nothing";

        //        // Column 1
        //        case "->":
        //            //_parent.Memory.Counter.Increment();
        //            _parent.setReceiver("LIST_N");
        //            return "Nothing";
        //        case "<-":
        //            //_parent.Memory.Counter.Decrement();
        //            _parent.setReceiver("LIST_N");
        //            return "Nothing";
        //        case "B/O":
        //            //_parent.Memory.Counter.Set(0);
        //            _parent.setReceiver("LIST_N");
        //            return "Nothing";

        //        default:
        //            return "Nothing";
        //    }                
        //}
    }
}
