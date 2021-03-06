namespace OxygenNotIncluded
{
    public class Config
    {
        public float MaxAir = 100f;
        public float AirDepletionRate = 0.05f; // percent per second
        public float AirRegenerationRate = 0.1f; // percent per second

        public bool AirDepletesOnDamageReceived = true; // lose air on damage received?

        public bool AirBarHorizontalAlignmentRight = true; // set to false to align the air bar to the left
        public float AirBarVerticalAlignmentOffset = -10.0f; // lesser number moves the bar up
    }
}
