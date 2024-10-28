public interface IEffectable
{
    void ApplyEffect() { }

    void RemoveEffect() { }
}

public interface IRevealable : IEffectable
{
    bool IsRevealed { get; set; }
}

public interface IHideable : IEffectable
{
    bool IsHidden { get; set; }
}

public interface IStunable : IEffectable 
{ 
    void ApplyStunEffect() { }
}


