using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerMVC
{
    public class PlayerController
    {
        private AnimationConfig _config;
        private SpriteAnimatorController _playerAnimator;
        private LevelObjectView _playerView;

        private Transform _playerT;

        private float _walkSpeed = 10f;
        private float _animationSpeed = 14f;
        private float _movingTrehold = 0.1f;
        private float _jumpForce = 8f;
        private float _jumpTrehold = 0.1f;
        private float _groundLevel = 0f;
        private float _g = -9.8f;
        private bool _isJump;
        private bool _isMoving;

        private Vector3 _leftScale = new Vector3(-1,1,1);
        private Vector3 _rightScale = new Vector3(1,1,1);

        private float _yVelocity = 0;
        private float _xAxisInput;


        public PlayerController(LevelObjectView playerView)
        {
            _playerView = playerView;
            _playerT = playerView._transform;

            _config = Resources.Load<AnimationConfig>("SpriteAnimCfg");
            _playerAnimator = new SpriteAnimatorController(_config);
            _playerAnimator.StartAnimation(_playerView._spriteRender, AnimState.Idle, true, _animationSpeed);
        }
        
        private void MoveTowards()
        {
            _playerT.position+=Vector3.right*(Time.deltaTime*_walkSpeed*(_xAxisInput<0?-1:1));
            _playerT.localScale = _xAxisInput<0?_leftScale:_rightScale;
        }

        bool IsGrounded()
        {
            return _playerT.position.y<=_groundLevel && _yVelocity<=0;
        }



        public void Update()
        {
            _xAxisInput = Input.GetAxis("Horizontal");
            _playerAnimator.Update();
            _isJump = Input.GetAxis("Vertical")>0;
            _isMoving = Mathf.Abs(_xAxisInput)>_movingTrehold;
            if(_isMoving)
            {
                MoveTowards();
            }
            _playerAnimator.StartAnimation(_playerView._spriteRender, _isMoving?AnimState.Run:AnimState.Idle, true, _animationSpeed);
            
            if(IsGrounded())
            {
                Debug.Log(_yVelocity);
                if(_isJump && _yVelocity<=0)
                {
                    _yVelocity = _jumpForce;
                }
                else if (_yVelocity<0)
                {
                    _yVelocity = 0;
                    _playerT.position = new Vector3(_playerT.position.x, _groundLevel, _playerT.position.z);
                }
            }
            else if(Mathf.Abs(_yVelocity)>_jumpTrehold)
            {
                _playerAnimator.StartAnimation(_playerView._spriteRender, AnimState.Jump, true, _animationSpeed);
            }
            _yVelocity+=_g*Time.deltaTime;
            _playerT.position+=Vector3.up*(Time.deltaTime*_yVelocity);
        }
    }
}