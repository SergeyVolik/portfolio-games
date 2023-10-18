using UnityEngine;

namespace Prototype
{
    public static class CombatConstants
    {
        public const float ATTACK_ANIMATION_HIT_TIMESTAMP = 0.45f; // strictly dependent of attack animation 
        public const int ATTACK_ANIMATOR_LAYER = 1;
        public const string ATTACK_ANIMATOR_NAME = "Attack";
        public const string EMPTY_ANIMATOR_NAME = "Empty";

        const string ATTACK_SPEED_FLOAT_PARAM = "attack_speed";
        const string MOVE_BOOL_PARAM = "move";
        const string ATTACK_TRIGGER_PARAM = "attack";
        const string DEATH_TRIGGER_PARAM = "death";
        const string IMPACT_TRIGGER_PARAM = "impact";

        public static readonly int ATTACK_SPEED_FLOAT_PARAM_HASH = Animator.StringToHash(ATTACK_SPEED_FLOAT_PARAM);
        public static readonly int MOVE_BOOL_PARAM_HASH = Animator.StringToHash(MOVE_BOOL_PARAM);
        public static readonly int ATTACK_TRIGGER_PARAM_HASH = Animator.StringToHash(ATTACK_TRIGGER_PARAM);
        public static readonly int DEATH_TRIGGER_PARAM_HASH = Animator.StringToHash(DEATH_TRIGGER_PARAM);
        public static readonly int IMPACT_TRIGGER_PARAM_HASH = Animator.StringToHash(IMPACT_TRIGGER_PARAM);

        // Show/hide weapon
        public const float ATTACK_ANIMATION_TOTAL_TIME = 1.5f; // should be equal to attack animation's "exit time" + "transition duration"
        public const float WEAPON_HIDE_DELAY_AFTER_LAST_ATTACK = 0.5f;
        public const float WEAPON_APPEARANCE_ANIMATION_TIME = 0.5f;
        
        public const float UNIT_ATTACK_DISTANCE = 2f;
        public const float UNIT_ATTACK_ANGLE = 135f;
    }   
}