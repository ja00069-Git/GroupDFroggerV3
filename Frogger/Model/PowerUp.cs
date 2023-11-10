namespace Frogger.Model
{
    /// <summary>
    ///     Players power up
    /// </summary>
    public class PowerUp
    {
        #region Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has double score effect.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has double score effect; otherwise, <c>false</c>.
        /// </value>
        public bool HasDoubleScoreEffect { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Activates this instance.
        /// </summary>
        public void Activate(bool hasDoubleScoreEffect)
        {
            this.IsActive = true;
            this.HasDoubleScoreEffect = hasDoubleScoreEffect;
        }

        /// <summary>
        ///     Deactivates this instance.
        /// </summary>
        public void Deactivate()
        {
            this.IsActive = false;
            this.HasDoubleScoreEffect = false;
        }

        #endregion
    }
}