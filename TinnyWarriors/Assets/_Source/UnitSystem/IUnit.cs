namespace UnitSystem
{
    public interface IUnit 
    {
        public int Damage { get; }
        public int MaxHp { get; }
        public int CurrHp { get; }
    }
}
