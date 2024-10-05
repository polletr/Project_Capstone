public interface IEffectable
{
    void ApplyEffect() { }

    void RemoveEffect() { }
}

public interface IMovable : IEffectable { }

public interface IRevealable : IEffectable { }

public interface IStunnable : IEffectable 
{ 
    void ApplyStunEffect() { }
}

public interface IParalisable : IEffectable { }



