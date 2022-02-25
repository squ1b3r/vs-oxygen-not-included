using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace OxygenNotIncluded.EntityBehavior
{
    public class EntityBehaviorAir : Vintagestory.API.Common.Entities.EntityBehavior
    {
        private ITreeAttribute _airTree;

        private float _secondsSinceLastUpdate;

        private float CurrentAir
        {
            get => _airTree.GetFloat("current-air");
            set
            {
                _airTree.SetFloat("current-air", GameMath.Clamp(value, 0, MaxAir));
                MarkDirty();
            }
        }

        private float MaxAir
        {
            get => _airTree.GetFloat("max-air");
            set
            {
                _airTree.SetFloat("max-air", value);
                MarkDirty();
            }
        }

        public EntityBehaviorAir(Entity entity) : base(entity)
        {
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            _airTree = entity.WatchedAttributes.GetTreeAttribute("oni:air");

            if (_airTree == null)
            {
                entity.WatchedAttributes.SetAttribute("oni:air", _airTree = new TreeAttribute());

                CurrentAir = attributes["current-air"].AsFloat(OxygenNotIncludedMod.Config.MaxAir);
                MaxAir = attributes["max-air"].AsFloat(OxygenNotIncludedMod.Config.MaxAir);

                MarkDirty();

                return;
            }

            CurrentAir = _airTree.GetFloat("current-air");
            MaxAir = _airTree.GetFloat("max-air");

            MarkDirty();
        }

        public override void OnGameTick(float deltaTime)
        {
            if (entity.State != EnumEntityState.Active) return;

            if (!(entity is EntityPlayer player)) return;

            if (!entity.Alive) return;

            var gameMode = player?.Player?.WorldData?.CurrentGameMode;

            if (gameMode != EnumGameMode.Survival) return;

            _secondsSinceLastUpdate += deltaTime;

            if (_secondsSinceLastUpdate < 1) return;

            _secondsSinceLastUpdate = 0;

            if (CanBreathe())
            {
                RegenerateAir();
            }
            else
            {
                ReduceAir(OxygenNotIncludedMod.Config.AirDepletionRate);
            }

            if (CurrentAir <= 0)
            {
                Suffocate();
            }
        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage)
        {
            base.OnEntityReceiveDamage(damageSource, ref damage);

            if (!OxygenNotIncludedMod.Config.AirDepletesOnDamageReceived) return;

            if (entity.World.Side != EnumAppSide.Server) return;

            if (damageSource.Source == EnumDamageSource.Drown) return;
            if (damageSource.Type == EnumDamageType.Suffocation) return;
            if (damageSource.Type == EnumDamageType.Heal) return;

            ReduceAir(OxygenNotIncludedMod.Config.AirDepletionRate * 2);
        }

        private void MarkDirty() => entity.WatchedAttributes.MarkPathDirty("oni:air");

        private void ReduceAir(float depletionRate) => CurrentAir -= MaxAir * depletionRate;

        private void RegenerateAir() => CurrentAir += MaxAir * OxygenNotIncludedMod.Config.AirRegenerationRate;

        private bool CanBreathe() => !IsUnderwater() && !IsBuried();

        private bool IsUnderwater() => entity.Swimming && !IsHeadBlockBreathable();

        private bool IsBuried() => entity.OnGround && !IsHeadBlockBreathable();

        /// <summary>
        /// Defines whether or not the head block is breathable
        /// </summary>
        /// <returns>
        /// Returns true if the block is air or is a none-solid block which is also not liquid
        /// </returns>
        private bool IsHeadBlockBreathable()
        {
            var head = entity.SidedPos.XYZ.AddCopy(0, entity.CollisionBox.Height, 0);
            var headBlock = entity.World.BlockAccessor.GetBlock(head.AsBlockPos);

            var isSolidBlock = !headBlock.SideSolid.Contains(false);
            var isLiquidBlock = headBlock.IsLiquid();

            return (headBlock.BlockMaterial == EnumBlockMaterial.Air || !isSolidBlock) && !isLiquidBlock;
        }

        private void Suffocate()
        {
            var damageSource = new DamageSource() {Type = EnumDamageType.Suffocation, Source = EnumDamageSource.Drown};
            entity.ReceiveDamage(damageSource, 1f);
        }

        public override string PropertyName() => "oni:air";
    }
}
