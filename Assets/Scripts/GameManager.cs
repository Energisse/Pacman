using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;

    public Pacman pacman;

    public Transform pellets;

    public int score { get; private set; }

    public int lives { get; private set; }

    // Start is called before the first frame update
    private void Start(){
        NewGame();
    }

    private void Update(){
        if(this.lives <= 0 &&  Input.anyKeyDown){
            NewGame();
        }
    }

    private void NewGame(){
        SetLives(3);
        SetScore(0);
        NewRound();
    }

    private void NewRound(){
        foreach (Transform pellet in this.pellets){
            this.pellet.gameObject.SetActive(true);
        }

        ResetRound();
    }

    private void ResetRound(){
        for(int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].gameObject.SetActive(true);
        }

        this.pacman.gameObject.SetActive(true);
    }

    private void GameOver(){
        for(int i = 0; i < this.ghosts.Length; i++){
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score){
        this.score = score;
    }

    private void SetLives(int lives){
        this.lives = lives;
    }

   

    public void GhostEaten(Ghost ghost){
        SetScore(this.score + ghost.points);
    }

    public void PacmanEaten(){
        this.pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);
        if(this.lives > 0){
            Invoke(nameof(ResetRound), 3.0f);
        }
        else{
            GameOver();
        }
    }
}
