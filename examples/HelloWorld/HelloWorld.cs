using DinoCat;
using DinoCat.Elements;
using DinoCat.State;
using static DinoCat.Elements.Factories;

App.Run(() =>
    State.Inject<int>((count, setCount) => new Row(
        Text("Hello World 🐱‍🐉")
            .Margin(2).Center(),
        Button(
            content: $"Clicked {count} time(s)!!",
            click: () => setCount(count + 1)))));