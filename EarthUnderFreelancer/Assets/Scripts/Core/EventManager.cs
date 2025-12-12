using UnityEngine;
using System;
using System.Collections.Generic;

namespace EarthUnderFreelancer.Core
{
    /// <summary>
    /// Central event system for decoupled communication between game systems
    /// </summary>
    public static class EventManager
    {
        private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

        public static void Subscribe(string eventName, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<object> existingEvent))
            {
                eventDictionary[eventName] = existingEvent + listener;
            }
            else
            {
                eventDictionary.Add(eventName, listener);
            }
        }

        public static void Unsubscribe(string eventName, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<object> existingEvent))
            {
                eventDictionary[eventName] = existingEvent - listener;
            }
        }

        public static void TriggerEvent(string eventName, object data = null)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent?.Invoke(data);
            }
        }

        public static void ClearAllEvents()
        {
            eventDictionary.Clear();
        }
    }

    /// <summary>
    /// Game event names as constants for type safety
    /// </summary>
    public static class GameEvents
    {
        public const string PLAYER_SPAWN = "PlayerSpawn";
        public const string PLAYER_DEATH = "PlayerDeath";
        public const string PLAYER_RESPAWN = "PlayerRespawn";
        public const string PLAYER_LEVEL_UP = "PlayerLevelUp";
        public const string PLAYER_CREDITS_CHANGED = "PlayerCreditsChanged";
        public const string PLAYER_EXPERIENCE_GAINED = "PlayerExperienceGained";

        public const string ENEMY_KILLED = "EnemyKilled";
        public const string DAMAGE_DEALT = "DamageDealt";
        public const string DAMAGE_TAKEN = "DamageTaken";
        public const string SHIELD_DEPLETED = "ShieldDepleted";
        public const string WEAPON_FIRED = "WeaponFired";
        public const string MISSILE_LOCKED = "MissileLocked";

        public const string MISSION_STARTED = "MissionStarted";
        public const string MISSION_COMPLETED = "MissionCompleted";
        public const string MISSION_FAILED = "MissionFailed";
        public const string OBJECTIVE_COMPLETED = "ObjectiveCompleted";
        public const string OBJECTIVE_UPDATED = "ObjectiveUpdated";

        public const string ITEM_PURCHASED = "ItemPurchased";
        public const string ITEM_SOLD = "ItemSold";
        public const string TRADE_COMPLETED = "TradeCompleted";

        public const string VEHICLE_UPGRADED = "VehicleUpgraded";
        public const string VEHICLE_EQUIPPED = "VehicleEquipped";
        public const string VEHICLE_CUSTOMIZED = "VehicleCustomized";

        public const string CONNECTED_TO_SERVER = "ConnectedToServer";
        public const string DISCONNECTED_FROM_SERVER = "DisconnectedFromServer";
        public const string PLAYER_JOINED = "PlayerJoined";
        public const string PLAYER_LEFT = "PlayerLeft";
        public const string MATCH_STARTED = "MatchStarted";
        public const string MATCH_ENDED = "MatchEnded";

        public const string SHOW_NOTIFICATION = "ShowNotification";
        public const string SHOW_POPUP = "ShowPopup";
        public const string UPDATE_HUD = "UpdateHUD";

        public const string AD_WATCHED = "AdWatched";
        public const string PURCHASE_COMPLETED = "PurchaseCompleted";
        public const string REWARD_EARNED = "RewardEarned";
    }

    [Serializable]
    public class DamageEventData
    {
        public GameObject source;
        public GameObject target;
        public float damage;
        public DamageType damageType;
        public Vector3 hitPoint;
    }

    public enum DamageType
    {
        Kinetic,
        Energy,
        Explosive,
        EMP
    }

    [Serializable]
    public class NotificationEventData
    {
        public string title;
        public string message;
        public NotificationType type;
        public float duration = 3f;
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
        Reward
    }

    [Serializable]
    public class MissionEventData
    {
        public string missionId;
        public string missionName;
        public int score;
        public int creditsEarned;
        public int experienceEarned;
    }
}
