using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour {
    
        public static void SaveScore(string playerName, int score) {
            List<HighScore> highScores = LoadHighScores();

            // Verifica se o jogador j� existe na lista de pontua��es
            bool playerExists = false;
            for (int i = 0; i < highScores.Count; i++) {
                if (highScores[i].name == playerName) {
                    // Se o jogador j� existe e a nova pontua��o for maior, atualiza a pontua��o
                    if (score > highScores[i].score) {
                        highScores[i].score = score;
                    }
                    playerExists = true;
                    break;
                }
            }

            // Se o jogador n�o existir, adiciona a nova pontua��o
            if (!playerExists) {
                highScores.Add(new HighScore(playerName, score));
            }

            // Ordena as pontua��es em ordem decrescente
            highScores.Sort((x, y) => y.score.CompareTo(x.score));

            // Mant�m apenas as 3 melhores pontua��es
            if (highScores.Count > 5) {
                highScores.RemoveAt(5); // Remove a menor pontua��o
            }

            // Salva os dados no PlayerPrefs
            for (int i = 0; i < highScores.Count; i++) {
                PlayerPrefs.SetString("TopPlayer" + i, highScores[i].name);
                PlayerPrefs.SetInt("TopScore" + i, highScores[i].score);
            }

            PlayerPrefs.Save();
        }


        public static List<HighScore> LoadHighScores() {
            List<HighScore> highScores = new List<HighScore>();

            // Checa se os dados existem e carrega as pontua��es
            for (int i = 0; i < 5; i++) {
                string name = PlayerPrefs.GetString("TopPlayer" + i, "");
                int score = PlayerPrefs.GetInt("TopScore" + i, 0);
                if (!string.IsNullOrEmpty(name)) {
                    highScores.Add(new HighScore(name, score));
                }
            }

            // Se n�o houver pontua��es, inicializa com valores padr�o (opcional)
            if (highScores.Count == 0) {
                highScores.Add(new HighScore("Player1", 0));
                highScores.Add(new HighScore("Player2", 0));
                highScores.Add(new HighScore("Player3", 0));
                highScores.Add(new HighScore("Player4", 0));
                highScores.Add(new HighScore("Player5", 0));
                SaveInitialScores(highScores);
            }

            return highScores;
        }

        private static void SaveInitialScores(List<HighScore> highScores) {
            for (int i = 0; i < highScores.Count; i++) {
                PlayerPrefs.SetString("TopPlayer" + i, highScores[i].name);
                PlayerPrefs.SetInt("TopScore" + i, highScores[i].score);
            }
            PlayerPrefs.Save();
        }

        public class HighScore {
            public string name;
            public int score;

            public HighScore(string name, int score) {
                this.name = name;
                this.score = score;
            }
        }
}