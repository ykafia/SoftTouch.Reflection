using SoftTouch.Reflection.Core;
using System.Runtime.CompilerServices;

namespace SoftTouch.Reflection.Example;

[Reflectable]
public partial struct Person
{
    public string Name { get; set; }
    public int Age { get; }
    public House PersonHouse { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

}

[Reflectable]
public partial class House
{
    public string Address { get; }
    internal int Number { get; set; }

}