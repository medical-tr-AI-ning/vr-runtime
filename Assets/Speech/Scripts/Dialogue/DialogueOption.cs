using System;

public class DialogueOption
{
    private readonly bool _hasResponse;

    // often there are several answer possibilities for a dialogue option. On start of the scenario one is selected for the run
    private readonly string _response;

    public bool HasResponse { get => this._hasResponse; }

    public string Response { 
        get {
            if (this._hasResponse)
            {
                return this._response;
            }
            else return null;
        } 
    }


    public DialogueOption(string[] responsePossibilities)
    {
        this._hasResponse = true;
        if (responsePossibilities != null)
        {
            Random r = new();
            int selectedResponseIndex = r.Next(0, responsePossibilities.Length);
            this._response = responsePossibilities[selectedResponseIndex];
        }
    }
}
