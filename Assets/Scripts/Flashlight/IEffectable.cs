public interface IEffectable
{
    void ApplyEffect() { }
}

public interface IMovable : IEffectable { }

public interface IRevealable : IEffectable { }

public interface IStunnable : IEffectable { }


