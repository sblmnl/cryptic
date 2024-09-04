module App

open System
open Feliz
open Feliz.UseElmish
open Elmish
open Browser.Dom

[<RequireQualifiedAccess>]
module Http =
    open Fable.SimpleHttp
    open Thoth.Json

    type CodedError = { Code: string; Message: string }

    type MetaLink = {
        Href: string
        Rel: string
        Method: string
    }

    type MetaInformation = { Links: MetaLink list }

    type HttpResponse<'a> = {
        Status: string
        Data: 'a option
        Errors: CodedError list
        Warnings: string list
        Meta: MetaInformation
    }

    type CreateNoteRequest = {
        Content: string
        Password: string option
        DeleteAfter: string
    }

    type CreateNoteResponse = {
        Id: Guid
        DeleteAfterTime: DateTimeOffset
        DeleteOnReceipt: bool
        ControlToken: string
    }

    let createNote req =
        async {
            let requestBody = req |> Encode.Auto.toString<CreateNoteRequest> |> BodyContent.Text

            let! response =
                Http.request "/api/notes"
                |> Http.method POST
                |> Http.content requestBody
                |> Http.header (Headers.contentType "application/json")
                |> Http.send

            return Decode.Auto.fromString<HttpResponse<CreateNoteResponse>> response.responseText
        }

module App =
    [<RequireQualifiedAccess>]
    type DeleteAfter =
        | Reading
        | AnHour
        | ADay
        | AWeek
        | AMonth

    [<RequireQualifiedAccess>]
    module DeleteAfter =
        let toString deleteAfter =
            match deleteAfter with
            | DeleteAfter.Reading -> "Reading"
            | DeleteAfter.AnHour -> "1 hour"
            | DeleteAfter.ADay -> "1 day"
            | DeleteAfter.AWeek -> "1 week"
            | DeleteAfter.AMonth -> "1 month"

        let fromString deleteAfter =
            match deleteAfter with
            | "Reading" -> Some DeleteAfter.Reading
            | "1 hour" -> Some DeleteAfter.AnHour
            | "1 day" -> Some DeleteAfter.ADay
            | "1 week" -> Some DeleteAfter.AWeek
            | "1 month" -> Some DeleteAfter.AMonth
            | _ -> None

    type Model = {
        Content: string
        Password: string option
        DeleteAfter: DeleteAfter
    }

    [<RequireQualifiedAccess>]
    type Msg =
        | SetNoteContent of string
        | SetPassword of string option
        | SetDeleteAfter of DeleteAfter
        | CreateNote
        | GotCreateNoteResult of Result<Http.HttpResponse<Http.CreateNoteResponse>, string>
        | GotCreateNoteException of exn

    let init () =
        {
            Content = ""
            Password = None
            DeleteAfter = DeleteAfter.Reading
        },
        Cmd.none

    let update msg model =
        match msg with
        | Msg.SetNoteContent content -> { model with Content = content }, Cmd.none
        | Msg.SetPassword password -> { model with Password = password }, Cmd.none
        | Msg.SetDeleteAfter deleteAfter -> { model with DeleteAfter = deleteAfter }, Cmd.none
        | Msg.CreateNote ->
            model,
            Cmd.OfAsync.either
                Http.createNote
                {
                    Content = model.Content
                    Password = model.Password
                    DeleteAfter = DeleteAfter.toString model.DeleteAfter
                }
                Msg.GotCreateNoteResult
                Msg.GotCreateNoteException
        | Msg.GotCreateNoteResult _ -> model, Cmd.none
        | Msg.GotCreateNoteException exn ->
            printfn "%s" exn.Message
            model, Cmd.none

    let stringToOption string =
        if String.IsNullOrWhiteSpace string then
            None
        else
            Some string

    [<ReactComponent>]
    let View () =
        let model, dispatch = React.useElmish (init, update)

        Html.main [
            prop.className "container"
            prop.children [
                Html.form [
                    prop.children [
                        Html.fieldSet [
                            prop.children [
                                Html.legend "Create a note"
                                Html.p [
                                    Html.textarea [
                                        prop.placeholder "Enter your note content..."
                                        prop.onChange (Msg.SetNoteContent >> dispatch)
                                    ]
                                ]
                                Html.p [
                                    Html.label [ prop.text "Password (optional):"; prop.htmlFor "password" ]
                                    Html.input [
                                        prop.name "password"
                                        prop.type'.password
                                        prop.onChange (stringToOption >> Msg.SetPassword >> dispatch)
                                    ]
                                ]
                                Html.p [
                                    Html.label [ prop.text "Delete after:"; prop.htmlFor "deleteAfter" ]
                                    Html.select [
                                        prop.name "deleteAfter"
                                        prop.children [
                                            for deleteAfter in
                                                [
                                                    DeleteAfter.Reading
                                                    DeleteAfter.AnHour
                                                    DeleteAfter.ADay
                                                    DeleteAfter.AWeek
                                                    DeleteAfter.AMonth
                                                ] do
                                                Html.option [
                                                    prop.text (DeleteAfter.toString deleteAfter)
                                                    prop.selected (model.DeleteAfter = deleteAfter)
                                                ]
                                        ]
                                        prop.onChange (
                                            DeleteAfter.fromString >> Option.iter (Msg.SetDeleteAfter >> dispatch)
                                        )
                                    ]
                                ]
                                Html.button [
                                    prop.type'.button
                                    prop.text "Create"
                                    prop.onClick (fun _ -> dispatch Msg.CreateNote)
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

ReactDOM.createRoot(document.getElementById "app").render (App.View())
