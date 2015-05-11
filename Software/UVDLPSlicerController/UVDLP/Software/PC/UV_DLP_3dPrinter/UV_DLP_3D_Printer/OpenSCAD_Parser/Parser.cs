using System.Collections;



using System;



public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _floatcon = 2;
	public const int _intcon = 3;
	public const int _string = 4;
	public const int _charcon = 5;
	public const int _auto = 6;
	public const int _case = 7;
	public const int _char = 8;
	public const int _const = 9;
	public const int _default = 10;
	public const int _double = 11;
	public const int _enum = 12;
	public const int _extern = 13;
	public const int _float = 14;
	public const int _int = 15;
	public const int _long = 16;
	public const int _register = 17;
	public const int _short = 18;
	public const int _signed = 19;
	public const int _static = 20;
	public const int _struct = 21;
	public const int _typedef = 22;
	public const int _union = 23;
	public const int _unsigned = 24;
	public const int _void = 25;
	public const int _volatile = 26;
	public const int _comma = 27;
	public const int _semicolon = 28;
	public const int _colon = 29;
	public const int _star = 30;
	public const int _lpar = 31;
	public const int _rpar = 32;
	public const int _lbrack = 33;
	public const int _rbrace = 34;
	public const int _ellipsis = 35;
	public const int maxT = 84;
	public const int _ppDefine = 85;
	public const int _ppUndef = 86;
	public const int _ppIf = 87;
	public const int _ppElif = 88;
	public const int _ppElse = 89;
	public const int _ppEndif = 90;
	public const int _ppInclude = 91;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public SymTab tab; // symbol table

//------------------ token sets ------------------------------------

static BitArray
	startOfTypeName = NewBitArray(_const, _volatile, _void, _char, _short, _int, _long,
		                _double, _signed, _unsigned, _struct, _union, _enum),
	startOfDecl     = NewBitArray(_typedef, _extern, _static, _auto, _register, _const, 
	                  _volatile, _void, _char, _short, _int, _long, _double, _signed, 
	                  _unsigned, _struct, _union, _enum);

private static BitArray NewBitArray(params int[] val) {
	BitArray s = new BitArray(128);
	foreach (int x in val) s[x] = true;
	return s;
}

//---------- LL(1) conflict resolvers ------------------------------

private bool IsTypeName(Token x) {
	if (x.kind != _ident) return false;
	Obj obj = tab.Find(x.val);
	return obj.kind == Obj.TYPE;
}

bool IsType0() { // return true if the next token is a type name
	return IsTypeName(la);
} 

bool IsType1() { // return true if "(" TypeName
	if (la.kind != _lpar) return false;
	Token x = scanner.Peek();
	if (startOfTypeName[x.kind]) return true;
	return IsTypeName(x);
}

bool Continued() { // return true if not "," "}"
	if (la.kind == _comma) {
		Token x = scanner.Peek();
		if (x.kind == _rbrace) return false;
	}
	return true; 
}

bool Continued1() { // return true if ",", which is not followed by "..."
	if (la.kind == _comma) {
		Token x = scanner.Peek();
		if (x.kind != _ellipsis) return true;
	}
	return false; 
}

bool IsLabel() { // return true if ident ":" | "case" | "default"
	if (la.kind == _ident) {
		Token x = scanner.Peek();
		if (x.kind == _colon) return true;
	} else if (la.kind == _case || la.kind == _default) {
		return true;
	}
	return false; 
}

bool IsDecl() { // return true if followed by Decl
	if (startOfDecl[la.kind]) return true;
	return IsTypeName(la);
}

bool IsAbstractDecl() { // return true if there is no non-type-ident after '*', '(', "const", "volatile"
	Token x = la;
	while (x.kind == _star || x.kind == _lpar || x.kind == _const || x.kind == _volatile) x = scanner.Peek();
	if (x.kind != _ident) return true;
	return IsTypeName(x);
}

bool IsDeclarator() { // return true if '*', '(', '[', ';', noTypeIdent
	if (la.kind == _star || la.kind == _lpar || la.kind == _lbrack || la.kind == _semicolon || la.kind == 0) return true;
	if (la.kind != _ident) return false;
	return !IsTypeName(la);
}




	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }
				if (la.kind == 85) {
				}
				if (la.kind == 86) {
				}
				if (la.kind == 87) {
				}
				if (la.kind == 88) {
				}
				if (la.kind == 89) {
				}
				if (la.kind == 90) {
				}
				if (la.kind == 91) {
				}

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void C() {
		ExternalDecl();
		while (StartOf(1)) {
			ExternalDecl();
		}
	}

	void ExternalDecl() {
		DeclSpecifierList();
		if (la.kind == 1 || la.kind == 30 || la.kind == 31) {
			Declarator();
			if (StartOf(2)) {
				while (StartOf(1)) {
					Decl();
				}
				Expect(36);
				while (StartOf(3)) {
					if (IsDecl()) {
						Decl();
					} else {
						Stat();
					}
				}
				Expect(34);
			} else if (la.kind == 27 || la.kind == 28 || la.kind == 37) {
				if (la.kind == 37) {
					Get();
					Initializer();
				}
				while (la.kind == 27) {
					Get();
					InitDeclarator();
				}
				Expect(28);
			} else SynErr(85);
		} else if (la.kind == 28) {
			Get();
		} else SynErr(86);
	}

	void DeclSpecifierList() {
		DeclSpecifier();
		while (!IsDeclarator()) {
			DeclSpecifier();
		}
	}

	void Declarator() {
		if (la.kind == 30) {
			Pointer();
		}
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 31) {
			Get();
			Declarator();
			Expect(32);
		} else SynErr(87);
		while (la.kind == 31 || la.kind == 33) {
			if (la.kind == 33) {
				Get();
				if (StartOf(4)) {
					ConstExpr();
				}
				Expect(38);
			} else {
				Get();
				if (StartOf(1)) {
					if (!IsType0()) {
						IdentList();
					} else {
						ParamTypeList();
					}
				}
				Expect(32);
			}
		}
	}

	void Decl() {
		DeclSpecifierList();
		if (la.kind == 1 || la.kind == 30 || la.kind == 31) {
			InitDeclarator();
			while (la.kind == 27) {
				Get();
				InitDeclarator();
			}
		}
		Expect(28);
	}

	void Stat() {
		if (IsLabel()) {
			if (la.kind == 1) {
				Get();
			} else if (la.kind == 7) {
				Get();
				ConstExpr();
			} else if (la.kind == 10) {
				Get();
			} else SynErr(88);
			Expect(29);
			Stat();
		} else if (StartOf(4)) {
			Expr();
			Expect(28);
		} else if (la.kind == 36) {
			Get();
			while (StartOf(3)) {
				if (IsDecl()) {
					Decl();
				} else {
					Stat();
				}
			}
			Expect(34);
		} else if (la.kind == 74) {
			Get();
			Expect(31);
			Expr();
			Expect(32);
			Stat();
			if (la.kind == 75) {
				Get();
				Stat();
			}
		} else if (la.kind == 76) {
			Get();
			Expect(31);
			Expr();
			Expect(32);
			Stat();
		} else if (la.kind == 77) {
			Get();
			Expect(31);
			Expr();
			Expect(32);
			Stat();
		} else if (la.kind == 78) {
			Get();
			Stat();
			Expect(77);
			Expect(31);
			Expr();
			Expect(32);
			Expect(28);
		} else if (la.kind == 79) {
			Get();
			Expect(31);
			if (IsDecl()) {
				Decl();
			} else if (StartOf(5)) {
				if (StartOf(4)) {
					Expr();
				}
				Expect(28);
			} else SynErr(89);
			if (StartOf(4)) {
				Expr();
			}
			Expect(28);
			if (StartOf(4)) {
				Expr();
			}
			Expect(32);
			Stat();
		} else if (la.kind == 80) {
			Get();
			Expect(1);
			Expect(28);
		} else if (la.kind == 81) {
			Get();
			Expect(28);
		} else if (la.kind == 82) {
			Get();
			Expect(28);
		} else if (la.kind == 83) {
			Get();
			if (StartOf(4)) {
				Expr();
			}
			Expect(28);
		} else if (la.kind == 28) {
			Get();
		} else SynErr(90);
	}

	void Initializer() {
		if (StartOf(4)) {
			AssignExpr();
		} else if (la.kind == 36) {
			Get();
			Initializer();
			while (Continued()) {
				Expect(27);
				Initializer();
			}
			if (la.kind == 27) {
				Get();
			}
			Expect(34);
		} else SynErr(91);
	}

	void InitDeclarator() {
		Declarator();
		if (la.kind == 37) {
			Get();
			Initializer();
		}
	}

	void DeclSpecifier() {
		switch (la.kind) {
		case 22: {
			Get();
			break;
		}
		case 13: {
			Get();
			break;
		}
		case 20: {
			Get();
			break;
		}
		case 6: {
			Get();
			break;
		}
		case 17: {
			Get();
			break;
		}
		case 9: {
			Get();
			break;
		}
		case 26: {
			Get();
			break;
		}
		case 1: case 8: case 11: case 12: case 14: case 15: case 16: case 18: case 19: case 21: case 23: case 24: case 25: {
			TypeSpecifier();
			break;
		}
		default: SynErr(92); break;
		}
	}

	void TypeSpecifier() {
		switch (la.kind) {
		case 25: {
			Get();
			break;
		}
		case 8: {
			Get();
			break;
		}
		case 18: {
			Get();
			break;
		}
		case 15: {
			Get();
			break;
		}
		case 16: {
			Get();
			break;
		}
		case 14: {
			Get();
			break;
		}
		case 11: {
			Get();
			break;
		}
		case 19: {
			Get();
			break;
		}
		case 24: {
			Get();
			break;
		}
		case 1: {
			Get();
			break;
		}
		case 21: case 23: {
			if (la.kind == 21) {
				Get();
			} else {
				Get();
			}
			if (la.kind == 1) {
				Get();
				if (la.kind == 36) {
					Get();
					StructDecl();
					while (StartOf(6)) {
						StructDecl();
					}
					Expect(34);
				}
			} else if (la.kind == 36) {
				Get();
				StructDecl();
				while (StartOf(6)) {
					StructDecl();
				}
				Expect(34);
			} else SynErr(93);
			break;
		}
		case 12: {
			Get();
			if (la.kind == 1) {
				Get();
				if (la.kind == 36) {
					Get();
					Enumerator();
					while (la.kind == 27) {
						Get();
						Enumerator();
					}
					Expect(34);
				}
			} else if (la.kind == 36) {
				Get();
				Enumerator();
				while (la.kind == 27) {
					Get();
					Enumerator();
				}
				Expect(34);
			} else SynErr(94);
			break;
		}
		default: SynErr(95); break;
		}
	}

	void StructDecl() {
		SpecifierQualifierList();
		StructDeclarator();
		while (la.kind == 27) {
			Get();
			StructDeclarator();
		}
		Expect(28);
	}

	void Enumerator() {
		Expect(1);
		if (la.kind == 37) {
			Get();
			ConstExpr();
		}
	}

	void SpecifierQualifierList() {
		if (StartOf(7)) {
			TypeSpecifier();
		} else if (la.kind == 9 || la.kind == 26) {
			TypeQualifier();
		} else SynErr(96);
		while (!IsDeclarator()) {
			if (StartOf(7)) {
				TypeSpecifier();
			} else if (la.kind == 9 || la.kind == 26) {
				TypeQualifier();
			} else SynErr(97);
		}
	}

	void StructDeclarator() {
		if (la.kind == 1 || la.kind == 30 || la.kind == 31) {
			Declarator();
			if (la.kind == 29) {
				Get();
				ConstExpr();
			}
		} else if (la.kind == 29) {
			Get();
			ConstExpr();
		} else SynErr(98);
	}

	void ConstExpr() {
		CondExpr();
	}

	void TypeQualifier() {
		if (la.kind == 9) {
			Get();
		} else if (la.kind == 26) {
			Get();
		} else SynErr(99);
	}

	void Pointer() {
		Expect(30);
		while (la.kind == 9 || la.kind == 26) {
			TypeQualifier();
		}
		while (la.kind == 30) {
			Get();
			while (la.kind == 9 || la.kind == 26) {
				TypeQualifier();
			}
		}
	}

	void IdentList() {
		Expect(1);
		while (la.kind == 27) {
			Get();
			Expect(1);
		}
	}

	void ParamTypeList() {
		ParamDecl();
		while (Continued1()) {
			Expect(27);
			ParamDecl();
		}
		if (la.kind == 27) {
			Get();
			Expect(35);
		}
	}

	void ParamDecl() {
		DeclSpecifierList();
		if (StartOf(8)) {
			if (IsAbstractDecl()) {
				AbstractDeclarator();
			} else {
				Declarator();
			}
		}
	}

	void AbstractDeclarator() {
		if (la.kind == 30) {
			Pointer();
			if (la.kind == 31 || la.kind == 33) {
				DirectAbstractDeclarator();
			}
		} else if (la.kind == 31 || la.kind == 33) {
			DirectAbstractDeclarator();
		} else SynErr(100);
	}

	void TypeName() {
		SpecifierQualifierList();
		if (la.kind == 30 || la.kind == 31 || la.kind == 33) {
			AbstractDeclarator();
		}
	}

	void DirectAbstractDeclarator() {
		if (la.kind == 31) {
			Get();
			if (StartOf(9)) {
				if (la.kind == 30 || la.kind == 31 || la.kind == 33) {
					AbstractDeclarator();
				} else {
					ParamTypeList();
				}
			}
			Expect(32);
		} else if (la.kind == 33) {
			Get();
			if (StartOf(4)) {
				ConstExpr();
			}
			Expect(38);
		} else SynErr(101);
		while (la.kind == 31 || la.kind == 33) {
			if (la.kind == 33) {
				Get();
				if (StartOf(4)) {
					ConstExpr();
				}
				Expect(38);
			} else {
				Get();
				if (StartOf(1)) {
					ParamTypeList();
				}
				Expect(32);
			}
		}
	}

	void AssignExpr() {
		CondExpr();
		if (StartOf(10)) {
			AssignOp();
			AssignExpr();
		}
	}

	void Expr() {
		AssignExpr();
		while (la.kind == 27) {
			Get();
			AssignExpr();
		}
	}

	void CondExpr() {
		LogOrExpr();
		if (la.kind == 39) {
			Get();
			Expr();
			Expect(29);
			CondExpr();
		}
	}

	void AssignOp() {
		switch (la.kind) {
		case 37: {
			Get();
			break;
		}
		case 64: {
			Get();
			break;
		}
		case 65: {
			Get();
			break;
		}
		case 66: {
			Get();
			break;
		}
		case 67: {
			Get();
			break;
		}
		case 68: {
			Get();
			break;
		}
		case 69: {
			Get();
			break;
		}
		case 70: {
			Get();
			break;
		}
		case 71: {
			Get();
			break;
		}
		case 72: {
			Get();
			break;
		}
		case 73: {
			Get();
			break;
		}
		default: SynErr(102); break;
		}
	}

	void LogOrExpr() {
		LogAndExpr();
		while (la.kind == 40) {
			Get();
			LogAndExpr();
		}
	}

	void LogAndExpr() {
		OrExpr();
		while (la.kind == 41) {
			Get();
			OrExpr();
		}
	}

	void OrExpr() {
		XorExpr();
		while (la.kind == 42) {
			Get();
			XorExpr();
		}
	}

	void XorExpr() {
		AndExpr();
		while (la.kind == 43) {
			Get();
			AndExpr();
		}
	}

	void AndExpr() {
		EqlExpr();
		while (la.kind == 44) {
			Get();
			EqlExpr();
		}
	}

	void EqlExpr() {
		RelExpr();
		while (la.kind == 45 || la.kind == 46) {
			if (la.kind == 45) {
				Get();
			} else {
				Get();
			}
			RelExpr();
		}
	}

	void RelExpr() {
		ShiftExpr();
		while (StartOf(11)) {
			if (la.kind == 47) {
				Get();
			} else if (la.kind == 48) {
				Get();
			} else if (la.kind == 49) {
				Get();
			} else {
				Get();
			}
			ShiftExpr();
		}
	}

	void ShiftExpr() {
		AddExpr();
		while (la.kind == 51 || la.kind == 52) {
			if (la.kind == 51) {
				Get();
			} else {
				Get();
			}
			AddExpr();
		}
	}

	void AddExpr() {
		MultExpr();
		while (la.kind == 53 || la.kind == 54) {
			if (la.kind == 53) {
				Get();
			} else {
				Get();
			}
			MultExpr();
		}
	}

	void MultExpr() {
		CastExpr();
		while (la.kind == 30 || la.kind == 55 || la.kind == 56) {
			if (la.kind == 30) {
				Get();
			} else if (la.kind == 55) {
				Get();
			} else {
				Get();
			}
			CastExpr();
		}
	}

	void CastExpr() {
		if (IsType1()) {
			Expect(31);
			TypeName();
			Expect(32);
			CastExpr();
		} else if (StartOf(4)) {
			UnaryExpr();
		} else SynErr(103);
	}

	void UnaryExpr() {
		while (la.kind == 57 || la.kind == 58) {
			if (la.kind == 57) {
				Get();
			} else {
				Get();
			}
		}
		if (StartOf(12)) {
			PostfixExpr();
		} else if (StartOf(13)) {
			UnaryOp();
			CastExpr();
		} else if (la.kind == 59) {
			Get();
			if (IsType1()) {
				Expect(31);
				TypeName();
				Expect(32);
			} else if (StartOf(4)) {
				UnaryExpr();
			} else SynErr(104);
		} else SynErr(105);
	}

	void PostfixExpr() {
		Primary();
		while (StartOf(14)) {
			switch (la.kind) {
			case 33: {
				Get();
				Expr();
				Expect(38);
				break;
			}
			case 60: {
				Get();
				Expect(1);
				break;
			}
			case 61: {
				Get();
				Expect(1);
				break;
			}
			case 31: {
				Get();
				if (StartOf(4)) {
					ArgExprList();
				}
				Expect(32);
				break;
			}
			case 57: {
				Get();
				break;
			}
			case 58: {
				Get();
				break;
			}
			}
		}
	}

	void UnaryOp() {
		switch (la.kind) {
		case 44: {
			Get();
			break;
		}
		case 30: {
			Get();
			break;
		}
		case 53: {
			Get();
			break;
		}
		case 54: {
			Get();
			break;
		}
		case 62: {
			Get();
			break;
		}
		case 63: {
			Get();
			break;
		}
		default: SynErr(106); break;
		}
	}

	void Primary() {
		switch (la.kind) {
		case 1: {
			Get();
			break;
		}
		case 3: {
			Get();
			break;
		}
		case 2: {
			Get();
			break;
		}
		case 5: {
			Get();
			break;
		}
		case 4: {
			Get();
			break;
		}
		case 31: {
			Get();
			Expr();
			Expect(32);
			break;
		}
		default: SynErr(107); break;
		}
	}

	void ArgExprList() {
		AssignExpr();
		while (la.kind == 27) {
			Get();
			AssignExpr();
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		C();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,x,T,T, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,T,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, T,T,T,T, x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,T,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,T,T,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, x,T,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, T,x,x,T, T,x,T,T, T,x,T,T, x,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "floatcon expected"; break;
			case 3: s = "intcon expected"; break;
			case 4: s = "string expected"; break;
			case 5: s = "charcon expected"; break;
			case 6: s = "auto expected"; break;
			case 7: s = "case expected"; break;
			case 8: s = "char expected"; break;
			case 9: s = "const expected"; break;
			case 10: s = "default expected"; break;
			case 11: s = "double expected"; break;
			case 12: s = "enum expected"; break;
			case 13: s = "extern expected"; break;
			case 14: s = "float expected"; break;
			case 15: s = "int expected"; break;
			case 16: s = "long expected"; break;
			case 17: s = "register expected"; break;
			case 18: s = "short expected"; break;
			case 19: s = "signed expected"; break;
			case 20: s = "static expected"; break;
			case 21: s = "struct expected"; break;
			case 22: s = "typedef expected"; break;
			case 23: s = "union expected"; break;
			case 24: s = "unsigned expected"; break;
			case 25: s = "void expected"; break;
			case 26: s = "volatile expected"; break;
			case 27: s = "comma expected"; break;
			case 28: s = "semicolon expected"; break;
			case 29: s = "colon expected"; break;
			case 30: s = "star expected"; break;
			case 31: s = "lpar expected"; break;
			case 32: s = "rpar expected"; break;
			case 33: s = "lbrack expected"; break;
			case 34: s = "rbrace expected"; break;
			case 35: s = "ellipsis expected"; break;
			case 36: s = "\"{\" expected"; break;
			case 37: s = "\"=\" expected"; break;
			case 38: s = "\"]\" expected"; break;
			case 39: s = "\"?\" expected"; break;
			case 40: s = "\"||\" expected"; break;
			case 41: s = "\"&&\" expected"; break;
			case 42: s = "\"|\" expected"; break;
			case 43: s = "\"^\" expected"; break;
			case 44: s = "\"&\" expected"; break;
			case 45: s = "\"==\" expected"; break;
			case 46: s = "\"!=\" expected"; break;
			case 47: s = "\"<\" expected"; break;
			case 48: s = "\">\" expected"; break;
			case 49: s = "\"<=\" expected"; break;
			case 50: s = "\">=\" expected"; break;
			case 51: s = "\"<<\" expected"; break;
			case 52: s = "\">>\" expected"; break;
			case 53: s = "\"+\" expected"; break;
			case 54: s = "\"-\" expected"; break;
			case 55: s = "\"/\" expected"; break;
			case 56: s = "\"%\" expected"; break;
			case 57: s = "\"++\" expected"; break;
			case 58: s = "\"--\" expected"; break;
			case 59: s = "\"sizeof\" expected"; break;
			case 60: s = "\".\" expected"; break;
			case 61: s = "\"->\" expected"; break;
			case 62: s = "\"~\" expected"; break;
			case 63: s = "\"!\" expected"; break;
			case 64: s = "\"*=\" expected"; break;
			case 65: s = "\"/=\" expected"; break;
			case 66: s = "\"%=\" expected"; break;
			case 67: s = "\"+=\" expected"; break;
			case 68: s = "\"-=\" expected"; break;
			case 69: s = "\"<<=\" expected"; break;
			case 70: s = "\">>=\" expected"; break;
			case 71: s = "\"&=\" expected"; break;
			case 72: s = "\"^=\" expected"; break;
			case 73: s = "\"|=\" expected"; break;
			case 74: s = "\"if\" expected"; break;
			case 75: s = "\"else\" expected"; break;
			case 76: s = "\"switch\" expected"; break;
			case 77: s = "\"while\" expected"; break;
			case 78: s = "\"do\" expected"; break;
			case 79: s = "\"for\" expected"; break;
			case 80: s = "\"goto\" expected"; break;
			case 81: s = "\"continue\" expected"; break;
			case 82: s = "\"break\" expected"; break;
			case 83: s = "\"return\" expected"; break;
			case 84: s = "??? expected"; break;
			case 85: s = "invalid ExternalDecl"; break;
			case 86: s = "invalid ExternalDecl"; break;
			case 87: s = "invalid Declarator"; break;
			case 88: s = "invalid Stat"; break;
			case 89: s = "invalid Stat"; break;
			case 90: s = "invalid Stat"; break;
			case 91: s = "invalid Initializer"; break;
			case 92: s = "invalid DeclSpecifier"; break;
			case 93: s = "invalid TypeSpecifier"; break;
			case 94: s = "invalid TypeSpecifier"; break;
			case 95: s = "invalid TypeSpecifier"; break;
			case 96: s = "invalid SpecifierQualifierList"; break;
			case 97: s = "invalid SpecifierQualifierList"; break;
			case 98: s = "invalid StructDeclarator"; break;
			case 99: s = "invalid TypeQualifier"; break;
			case 100: s = "invalid AbstractDeclarator"; break;
			case 101: s = "invalid DirectAbstractDeclarator"; break;
			case 102: s = "invalid AssignOp"; break;
			case 103: s = "invalid CastExpr"; break;
			case 104: s = "invalid UnaryExpr"; break;
			case 105: s = "invalid UnaryExpr"; break;
			case 106: s = "invalid UnaryOp"; break;
			case 107: s = "invalid Primary"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
