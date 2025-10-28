using System;
using UnityEngine;

public class StartAnimationTest : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _animator.Play("Attack1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
