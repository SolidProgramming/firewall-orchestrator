﻿using System;

namespace FWO.Report.Filter
{
    public enum TokenKind
    {
        Value,
        Source,
        Destination,
        Service,
        Protocol,
        DestinationPort,
        Action,
        Management,
        Gateway,
        Time,
        FullText,
        BL, // (
        BR, // )
        And,
        Or,
        Not,
        EQ, // ==
        NEQ // !=
    }
}