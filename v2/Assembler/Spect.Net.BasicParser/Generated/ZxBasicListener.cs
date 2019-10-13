//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\dotne\source\repos\spectnetide\v2\Assembler\AntlrZxBasicParserGenerator\AntlrZxBasicParserGenerator\ZxBasic.g4 by ANTLR 4.6.4

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Spect.Net.BasicParser.Generated {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="ZxBasicParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.4")]
[System.CLSCompliant(false)]
public interface IZxBasicListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompileUnit([NotNull] ZxBasicParser.CompileUnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.compileUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompileUnit([NotNull] ZxBasicParser.CompileUnitContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabel([NotNull] ZxBasicParser.LabelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabel([NotNull] ZxBasicParser.LabelContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLine([NotNull] ZxBasicParser.LineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLine([NotNull] ZxBasicParser.LineContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.line_item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLine_item([NotNull] ZxBasicParser.Line_itemContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.line_item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLine_item([NotNull] ZxBasicParser.Line_itemContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.asm_section"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAsm_section([NotNull] ZxBasicParser.Asm_sectionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.asm_section"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAsm_section([NotNull] ZxBasicParser.Asm_sectionContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.console"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConsole([NotNull] ZxBasicParser.ConsoleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.console"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConsole([NotNull] ZxBasicParser.ConsoleContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.keyword"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterKeyword([NotNull] ZxBasicParser.KeywordContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.keyword"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitKeyword([NotNull] ZxBasicParser.KeywordContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunction([NotNull] ZxBasicParser.FunctionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunction([NotNull] ZxBasicParser.FunctionContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOperator([NotNull] ZxBasicParser.OperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOperator([NotNull] ZxBasicParser.OperatorContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.special"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSpecial([NotNull] ZxBasicParser.SpecialContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.special"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSpecial([NotNull] ZxBasicParser.SpecialContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumber([NotNull] ZxBasicParser.NumberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumber([NotNull] ZxBasicParser.NumberContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdentifier([NotNull] ZxBasicParser.IdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdentifier([NotNull] ZxBasicParser.IdentifierContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString([NotNull] ZxBasicParser.StringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString([NotNull] ZxBasicParser.StringContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] ZxBasicParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] ZxBasicParser.TypeContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComment([NotNull] ZxBasicParser.CommentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComment([NotNull] ZxBasicParser.CommentContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.block_comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock_comment([NotNull] ZxBasicParser.Block_commentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.block_comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock_comment([NotNull] ZxBasicParser.Block_commentContext context);

	/// <summary>
	/// Enter a parse tree produced by <see cref="ZxBasicParser.line_comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLine_comment([NotNull] ZxBasicParser.Line_commentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="ZxBasicParser.line_comment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLine_comment([NotNull] ZxBasicParser.Line_commentContext context);
}
} // namespace Spect.Net.BasicParser.Generated