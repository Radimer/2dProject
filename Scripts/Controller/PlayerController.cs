using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerMVC
{
    public class PlayerController
    {
        private AnimationConfig _config;
        private SpriteAnimatorController _playerAnimator;
        private ContactPooler _contactPooler;
        private LevelObjectView _playerView;

        private Transform _playerT;
        private Rigidbody2D _rb;

        private float _walkSpeed = 150f;
        private float _animationSpeed = 14f;
        private float _movingTrehold = 0.1f;
        private float _jumpForce = 2f;
        private float _jumpTrehold = 0.1f;
        // private float _groundLevel = 0f;
        // private float _g = -9.8f;
        private bool _isJump;
        private bool _isMoving;

        private Vector3 _leftScale = new Vector3(-1,1,1);
        private Vector3 _rightScale = new Vector3(1,1,1);

        private float _yVelocity = 0;
        private float _xVelocity = 0;
        private float _xAxisInput;


        public PlayerController(LevelObjectView playerView)
        {
            _playerView = playerView;
            _playerT = playerView._transform;
            _rb = playerView._rb;

            _config = Resources.Load<AnimationConfig>("SpriteAnimCfg");
            _playerAnimator = new SpriteAnimatorController(_config);
            _playerAnimator.StartAnimation(_playerView._spriteRender, AnimState.Idle, true, _animationSpeed);
            _contactPooler = new ContactPooler(_playerView._collider);
        }
        
        private void MoveTowards()
        {
            _xVelocity = Time.fixedDeltaTime * _walkSpeed * (_xAxisInput<0?-1:1);
            _rb.velocity = new Vector2(_xVelocity, _yVelocity);
            _playerT.localScale = _xAxisInput<0?_leftScale:_rightScale;
        }

        public void Update()
        {
            _xAxisInput = Input.GetAxis("Horizontal");
            _playerAnimator.Update();
            _contactPooler.Update();
            _isJump = Input.GetAxis("Vertical")>0;
            _yVelocity = _rb.velocity.y;
            _isMoving = Mathf.Abs(_xAxisInput)>_movingTrehold;
            _playerAnimator.StartAnimation(_playerView._spriteRender, _isMoving?AnimState.Run:AnimState.Idle, true, _animationSpeed);

            if(_isMoving)
            {
                MoveTowards();
            }
            else
            {
                _xVelocity = 0;
                _rb.velocity = new Vector2(_xVelocity, _rb.velocity.y);
            }
            
            if(_contactPooler.IsGrounded)
            {
                if(_isJump && _yVelocity<=_jumpTrehold)
                {
                    _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                }
            }
            else if(Mathf.Abs(_yVelocity)>_jumpTrehold)
            {
                _playerAnimator.StartAnimation(_playerView._spriteRender, AnimState.Jump, true, _animationSpeed);
            }
        }
    }
}