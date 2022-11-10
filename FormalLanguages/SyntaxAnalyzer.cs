using System;
using System.Collections.Generic;

namespace FormalLanguages
{
	public class SyntaxAnalyzer
	{
		private List<Lexemee> _lexemeList;
		private IEnumerator<Lexemee> _lexemeEnumerator;

		public bool Run(string code)
		{
			LexicalAnalyser analyser = new();
			var result = analyser.Run(string.Join(Environment.NewLine, code));
			if (!result)
			{
				throw new Exception("Errors were occurred in lexical analyze");
			}

			return IsDoWhileStatement(analyser.Lexemes);


		}

		private bool IsDoWhileStatement(List<Lexemee> lexemeList)
		{
			_lexemeList = lexemeList;
			if (lexemeList.Count == 0) return false;

			_lexemeEnumerator = lexemeList.GetEnumerator();

			if (!_lexemeEnumerator.MoveNext() || _lexemeEnumerator.Current.Type != LexemeTypes.Do) { ErrorType.Error("Ожидается do", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.While) { ErrorType.Error("Ожидается while", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

			if (!IsCondition()) return false;
			//_lexemeEnumerator.MoveNext();

			while (IsStatement()) ;

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.Loop) { ErrorType.Error("Ожидается loop", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
			_lexemeEnumerator.MoveNext();

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
			}
			return true;
		}

		private bool RelationalExpression()
		{
			if (!IsOperand()) return false;
			if (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeTypes.Relation)
			{
				_lexemeEnumerator.MoveNext();
				if (!IsOperand()) return false;
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
					return true;
				}

				ErrorType.Error("Ожидается переменная", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}
			_lexemeEnumerator.MoveNext();

			if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeTypes.Assignment)
			{
				ErrorType.Error("Ожидается присваивание", _lexemeList.IndexOf(_lexemeEnumerator.Current));
				return false;
			}
			_lexemeEnumerator.MoveNext();

			if (!IsArithmeticExpression()) return false;

			return true;
		}

		private bool IsArithmeticExpression()
		{
			if (!IsOperand()) return false;
			while (_lexemeEnumerator.Current.Type == LexemeTypes.ArithmeticOperation)
			{
				_lexemeEnumerator.MoveNext();
				if (!IsOperand()) return false;
			}
			return true;
		}
	}
}

