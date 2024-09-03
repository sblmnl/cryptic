open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom

module App =
    type Model = { Count: int }

    [<RequireQualifiedAccess>]
    type Msg =
        | Increment
        | Decrement

    let init () = { Count = 0 }, Cmd.none

    let update msg model =
        match msg with
        | Msg.Increment -> { model with Count = model.Count + 1 }, Cmd.none
        | Msg.Decrement -> { model with Count = model.Count - 1 }, Cmd.none

    [<ReactComponent>]
    let View () =
        let model, dispatch = React.useElmish (init, update)

        Html.div [
            Html.p $"Count: {model.Count}"
            Html.button [ prop.text "Increment"; prop.onClick (fun _ -> dispatch Msg.Increment) ]
            Html.button [ prop.text "Decrement"; prop.onClick (fun _ -> dispatch Msg.Decrement) ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (App.View())
