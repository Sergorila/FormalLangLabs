using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormalLanguages
{
    public enum LexemeTypes { Do, Loop, While, Not, And, Or, Input, Output, Relation, ArithmeticOperation, Assignment, Undefined }

    public enum LexemeClases { Keyword, Identifier, Constant, SpecialSymbols, Undefined }

    public enum EntryType { Cmd, Var, Const, CmdPtr }

    public enum Cmd { JMP, JZ, SET, ADD, SUB, MUL, DIV, AND, OR, CMPE, CMPNE, CMPL, CMPLE, CMPG, CMPGE, OUTPUT, INPUT }

}
