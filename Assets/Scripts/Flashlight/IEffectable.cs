public interface IEffectable
{
    void ApplyEffect() { }

    void RemoveEffect() { }
}

public interface IMovable : IEffectable { }

public interface IRevealable : IEffectable
{
    bool IsRevealed { get; set; }
}

public interface IStunnable : IEffectable 
{ 
    void ApplyStunEffect() { }
}


