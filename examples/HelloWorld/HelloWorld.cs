using DinoCat;
using DinoCat.Elements;
using DinoCat.State;

App.Run(() =>
    State.Inject<int>((count, setCount) => new Row(
        new Text("Hello World!!")
            .Margin(2).Center(),
        new Button(
            content: $"Clicked {count} time(s)!!",
            click: () => setCount(count + 1)))));