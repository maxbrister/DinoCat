using DinoCat;
using DinoCat.Elements;
using DinoCat.State;

namespace HelloWorld
{
    class HelloWorld
    {
        public static void Main(string[] args) =>
            App.Run(SayHelloWorld);

        static Element SayHelloWorld() =>
            State.Inject<int>((count, setCount) => new Row(
                new Text("Hello World!!")
                    .Margin(2).Center(),
                new Button(
                    content: $"Clicked {count} time(s)!!",
                    click: () => setCount(count + 1))));
    }
}
