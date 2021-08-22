using UnityEngine;
using UnityEngine.AI;

namespace SystemExample.Entities
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] float movementSpeed = 5f;
        NavMeshAgent navMeshAgent;
        Animator anim;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
        }

        public void Move(Vector3 movement)
        {
            anim.SetFloat("Speed", 1);
            navMeshAgent.Move(movement * movementSpeed);
        }

        public float GetSpeed() => anim.GetFloat("Speed");
        public void SetSpeed(float spd) {
            anim.SetFloat("Speed", spd);
        } 
    }
}

