using System.Collections.Generic;
using System.Linq;

/*
    == Limited SQL Grammar ==

    S -> select TARGET_COLUMNS from TARGET_TABLE WHERE_CLAUSE

    TARGET_COLUMNS -> '*'
    TARGET_COLUMNS -> OBJECT_LIST

    OBJECT_LIST -> object OBJECT_LIST_NEXT

    OBJECT_LIST_NEXT -> ',' object OBJECT_LIST_NEXT
    OBJECT_LIST_NEXT -> ''

    TARGET_TABLE -> object

    WHERE_CLAUSE -> where CONDITION
    WHERE_CLAUSE -> ''

    CONDITION -> '(' CONDITION ')'
    CONDITION -> object operator CONDITION_TARGET CONDITION_NEXT

    CONDITION_NEXT -> and CONDITION
    CONDITION_NEXT -> or CONDITION
    CONDITION_NEXT -> ''

    CONDITION_TARGET -> object
    CONDITION_TARGET -> VALUE

    VALUE -> string_literal
    VALUE -> integer
 */
internal class Parser
{
    private readonly Stack<Token> _tokens;

    private Token _current;
    private Token Current => _current;
    private Token Lookahead => _tokens.Any() ? _tokens.Peek() : null;

    private void Next()
    {
        _current = _tokens.Any() ? _tokens.Pop() : null;
    }
    
    public Parser(IEnumerable<Token> tokens)
    {
        _tokens = new Stack<Token>();
        foreach (var token in tokens.Reverse())
        {
            if (token.Type == TokenType.Whitespace) continue;
            _tokens.Push(token);
        }

    }

    public QueryModel Parse()
    {
        Next();
        if (Current?.Type == TokenType.SelectKeyword)
        {
            return ParseSelect();
        }

        return null;   
    }

    // SELECT {*|[obj,]} FROM obj [WHERE obj op {string|int}]
    private QueryModel ParseSelect()
    {
        var queryModel = new QueryModel();
        Next();
        if (Current?.Type == TokenType.Object)
        {
            var targetColumns = ParseObjectList();
            queryModel.TargetColumns = targetColumns;
        }
        else
        {
            throw new ParserException("Expected column list");
        }

        if (Current?.Type == TokenType.FromKeyword)
        {
            Next();
        }
        else
        {
            throw new ParserException("Expected FROM keyword");
        }

        if (Current?.Type == TokenType.Object)
        {
            queryModel.TargetTable = Current?.Value;
        }
        else
        {
            throw new ParserException("Expected target table");
        }

        return queryModel;
    }

    private IList<string> ParseObjectList()
    {
        var objectList = new List<string>();
        objectList.Add(Current?.Value);

        Next();

        ParseObjectListNext(objectList);

        return objectList;
    }

    private void ParseObjectListNext(IList<string> objectList)
    {
        if (Current?.Type != TokenType.Comma) return;

        Next();
        objectList.Add(Current?.Value);
        Next();

        ParseObjectListNext(objectList);
    }
}