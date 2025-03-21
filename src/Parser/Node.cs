using TSqlFormatter.Parser;

namespace TSqlFormatter.Parser
{
    public class Node 
    {
        Token Token {get; set;}
        Node Prev {get; set;}
        List<Node> Children {get; set;}

    }
}