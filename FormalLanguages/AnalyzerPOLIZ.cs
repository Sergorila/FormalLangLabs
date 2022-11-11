﻿using System;
using System.Collections.Generic;

namespace FormalLanguages
{
	class AnalyzerPOLIZ
	{
		private List<Lexemee> _lexemeList;
		private IEnumerator<Lexemee> _lexemeEnumerator;

		private List<Entry> EntryList { get; set; }

		public bool Run(string code, out List<Entry> postfixEntries)
		{
			EntryList = new List<Entry>();

			LexicalAnalyser analyser = new();
			var result = analyser.Run(string.Join(Environment.NewLine, code));
			if (!result)
			{
				throw new Exception("Errors were occurred in lexical analyze");
			}

			bool res = IsDoWhileStatement(analyser.Lexemes);
			postfixEntries = new(EntryList);
			return res;
		}

		private bool IsDoWhileStatement(List<Lexemee> lexemeList)
		{
			var indFirst = EntryList.Count;
			_lexemeList = lexemeList;
			if (lexemeList.Count == 0) return false;

			_lexemeEnumerator = lexemeList.GetEnumerator();

			if (!_lexemeEnumerator.MoveNext() || _lexemeEnumerator.Current.Type != LexemeTypes.Do) { ErrorType.Error("Ожидается do", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.While) { ErrorType.Error("Ожидается while", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

			if (!IsCondition()) return false;

			var indJmpExit = WriteCmdPtr(-1);
			WriteCmd(Cmd.JZ);

			while (IsStatement()) ;

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.Loop) { ErrorType.Error("Ожидается loop", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

			WriteCmdPtr(indFirst);
			var indLast = WriteCmd(Cmd.JMP);
			SetCmdPtr(indJmpExit, indLast + 1);


			if (_lexemeEnumerator.MoveNext()) { ErrorType.Error("Лишние символы", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			return true;
		}

		private bool IsCondition()
		{
			if (!IsLogicalExpression()) return false;
			while (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeTypes.Or)
			{
				_lexemeEnumerator.MoveNext();
				if (!IsLogicalExpression()) return false;

				WriteCmd(Cmd.OR);
			}
			return true;
		}

		private bool IsLogicalExpression()
		{
			if (!RelationalExpression()) return false;
			while (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeTypes.And)
			{
				_lexemeEnumerator.MoveNext();
				if (!RelationalExpression()) return false;

				WriteCmd(Cmd.AND);
			}
			return true;
		}

		private bool RelationalExpression()
		{
			if (!IsOperand()) return false;
			if (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeTypes.Relation)
			{
				var cmd = _lexemeEnumerator.Current.Value switch
				{
					"<" => Cmd.CMPL,
					"<=" => Cmd.CMPLE,
					">" => Cmd.CMPG,
					">=" => Cmd.CMPGE,
					"==" => Cmd.CMPE,
					"<>" => Cmd.CMPNE,
					_ => throw new ArgumentException(_lexemeEnumerator.Current.Value)
				};

				_lexemeEnumerator.MoveNext();
				if (!IsOperand()) return false;

				WriteCmd(cmd);
			}
			return true;
		}

		private bool IsIdentifier()
		{
			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Class != LexemeClases.Identifier)
			{
				ErrorType.Error("Ожидается переменная", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}

			WriteVar(_lexemeList.IndexOf(_lexemeEnumerator.Current));

			_lexemeEnumerator.MoveNext();
			return true;
		}

		private bool IsOperand()
		{
			if (_lexemeEnumerator.Current == null || (_lexemeEnumerator.Current.Class != LexemeClases.Identifier && _lexemeEnumerator.Current.Class != LexemeClases.Constant))
			{
				ErrorType.Error("Ожидается переменная или константа", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}

			if (_lexemeEnumerator.Current.Class == LexemeClases.Identifier)
			{
				WriteVar(_lexemeList.IndexOf(_lexemeEnumerator.Current));
			}
			else
			{
				WriteConst(_lexemeList.IndexOf(_lexemeEnumerator.Current));
			}

			_lexemeEnumerator.MoveNext();
			return true;
		}

		private bool IsLogicalOperation()
		{
			if (_lexemeEnumerator.Current == null || (_lexemeEnumerator.Current.Type != LexemeTypes.And && _lexemeEnumerator.Current.Type != LexemeTypes.Or))
			{
				ErrorType.Error("Ожидается логическая операция", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}
			_lexemeEnumerator.MoveNext();
			return true;
		}

		private bool IsStatement()
		{
			if (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeTypes.Loop) return false;

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Class != LexemeClases.Identifier)
			{
				if (_lexemeEnumerator.Current.Type == LexemeTypes.Output)
				{
					_lexemeEnumerator.MoveNext();
					if (!IsOperand()) return false;

					WriteCmd(Cmd.OUTPUT);

					return true;
				}
				ErrorType.Error("Ожидается переменная", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}

			WriteVar(_lexemeList.IndexOf(_lexemeEnumerator.Current));

			_lexemeEnumerator.MoveNext();

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.Assignment)
			{
				ErrorType.Error("Ожидается присваивание", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}
			_lexemeEnumerator.MoveNext();

			if (!IsArithmeticExpression()) return false;

			WriteCmd(Cmd.SET);

			return true;
		}

		private bool IsArithmeticExpression()
		{
			if (!IsOperand()) return false;
			while (_lexemeEnumerator.Current.Type == LexemeTypes.ArithmeticOperation)
			{
				var cmd = _lexemeEnumerator.Current.Value switch
				{
					"+" => Cmd.ADD,
					"-" => Cmd.SUB,
					"*" => Cmd.MUL,
					"/" => Cmd.DIV,
					_ => throw new ArgumentException(_lexemeEnumerator.Current.Value)
				};

				_lexemeEnumerator.MoveNext();
				if (!IsOperand()) return false;

				WriteCmd(cmd);
			}
			return true;
		}

		private int WriteCmd(Cmd cmd)
		{
			var command = new Entry
			{
				EntryType = EntryType.Cmd,
				Cmd = cmd,
			};
			EntryList.Add(command);
			return EntryList.Count - 1;
		}

		private int WriteVar(int index)
		{
			var variable = new Entry
			{
				EntryType = EntryType.Var,
				Value = _lexemeList[index].Value
			};
			EntryList.Add(variable);
			return EntryList.Count - 1;
		}

		private int WriteConst(int index)
		{
			var variable = new Entry
			{
				EntryType = EntryType.Const,
				Value = _lexemeList[index].Value
			};
			EntryList.Add(variable);
			return EntryList.Count - 1;
		}

		private int WriteCmdPtr(int ptr)
		{
			var cmdPtr = new Entry
			{
				EntryType = EntryType.CmdPtr,
				CmdPtr = ptr,
			};
			EntryList.Add(cmdPtr);
			return EntryList.Count - 1;
		}

		private void SetCmdPtr(int index, int ptr)
		{
			EntryList[index].CmdPtr = ptr;
		}
	}
}
