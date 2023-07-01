// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PunPlayerScores.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
//  Scoring system for PhotonPlayer
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MFPS
{
    /// <summary>
    /// Scoring system for PhotonPlayer
    /// </summary>
    public class MyPunTeamScores : MonoBehaviour
    {
        public const string PlayerScoreProp = "_TeamScore";
    }

    public static class ScoreExtensions
    {
        public static void SetScore(this Player player, int newScore)
        {
            var key = MyPunTeamScores.PlayerScoreProp;
            Hashtable properties = new Hashtable(); // using PUN's implementation of Hashtable
            var scores = new int[2];
            scores[player.GetPhotonTeamCode()-1] = newScore;
            properties[key] = scores;

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            // this locally sets the score and will sync it in-game asap.
        }

        public static void AddScore(this Player player, int scoreToAddToCurrent)
        {
            var key = MyPunTeamScores.PlayerScoreProp;
            int[] scores = GetScores();
            scores[player.GetPhotonTeamCode()-1] += scoreToAddToCurrent;

            Hashtable properties = new Hashtable(); // using PUN's implementation of Hashtable
            properties[key] = scores;

            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            // this locally sets the score and will sync it in-game asap.
        }

        public static int[] GetScores()
        {
            var key = MyPunTeamScores.PlayerScoreProp;
            object score;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out score))
            {
                return (int[])score;
            }

            return new[] { 0, 0};
        }
    }
}