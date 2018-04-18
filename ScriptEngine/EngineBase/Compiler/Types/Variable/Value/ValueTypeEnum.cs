﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public enum ValueTypeEnum
    {
        NULL,
        DATE,
        STRING,
        NUMBER,
        BOOLEAN,
        SCRIPT_OBJECT,
        OBJECT,
        GUID,
        TYPE
    }
}
