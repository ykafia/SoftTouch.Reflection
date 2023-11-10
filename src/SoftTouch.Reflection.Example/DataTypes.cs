using SoftTouch.Reflection.Core;

namespace SoftTouch.Reflection.Example;

[Reflectable]
public partial struct Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}