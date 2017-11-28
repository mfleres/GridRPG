using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GridRPG
{
    /// <summary>
    /// Handles sprite animations and sheets.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimationManager : MonoBehaviour
    {
        private List<Tuple<List<Sprite>,int>> animationList = new List<Tuple<List<Sprite>, int>>(); //Each animation is composed of a list of sprites and the rate of display.
        private int currentAnimationId = -1;
        private int currentSpriteId = 0;
        private float lastUpdateTime = 0;
        private float translateSpeed = 0;
        private Vector2 translateDestination = Vector2.zero;

        public delegate void AnimationDoneEvent(int id);
        public AnimationDoneEvent animationDone;

        public delegate void DestinationReachedEvent(int id);
        public DestinationReachedEvent destinationReached;

        public int CurrentAnimationId
        {          
            get
            {
                return currentAnimationId;
            }
            set
            {
                //Debug.Log("AnimationManager: Changing animationID...");
                if (value < animationList.Count && value >= 0)
                {
                    //Change animation
                    //Debug.Log("AnimationManager: Changing animation to ID: " + value);
                    currentAnimationId = value;
                    currentSpriteId = 0;
                    lastUpdateTime = Time.fixedTime;
                    gameObject.GetComponent<SpriteRenderer>().sprite = animationList[currentAnimationId].Item1[currentSpriteId];
                }
                else if(value == -1)
                {
                    //Remove animation and current sprite
                    currentAnimationId = value;
                    currentSpriteId = 0;
                    lastUpdateTime = Time.fixedTime;
                    gameObject.GetComponent<SpriteRenderer>().sprite = null;
                }
                else
                {
                    Debug.Log("Animation Manager Error: ID out of range.");
                }
            }
        }

        /// <summary>
        /// Adds an animation to the animation manager.
        /// </summary>
        /// <param name="spriteSheet">Source to pull the sprites from.</param>
        /// <param name="spriteDims">Size of each sprite on the sheet.</param>
        /// <param name="sheetDims">Number of sprites in each dimension.</param>
        /// <param name="spriteOrder">Order to cycle through the sprites.</param>
        /// <param name="cycleRate">Number of sprites per second. 0 disables animation</param>
        /// <returns>ID of the animation in the manager.</returns>
        public int addAnimation(Texture2D spriteSheet,Vector2 spriteDims,Vector2 sheetDims,List<int> spriteOrder, int cycleRate)
        {
            //Debug.Log("AnimationManager.addAnimation(...): start.");
            //Record position
            int animationId = animationList.Count;
            //Debug.Log("AnimationManager.addAnimation(...): Return ID = "+animationId);
            //Create new animation
            List<Sprite> animation = new List<Sprite>();

            foreach (int spriteId in spriteOrder){
                //Add each sprite to the animation
                Rect spriteRect = new Rect(spriteDims.x * (spriteId % (int)sheetDims.x), spriteDims.y * (spriteId / (int)sheetDims.x), spriteDims.x, spriteDims.y);
                //Debug.Log("AnimationManager.addAnimation(...): spriteRect = " + spriteRect);
                animation.Add(Sprite.Create(spriteSheet, spriteRect, new Vector2(0.5f, 0.5f)));
            }

            animationList.Add(new Tuple<List<Sprite>, int>(animation,cycleRate));
            //Debug.Log("AnimationManager.addAnimation(...): end.");
            return animationId;
        }

        /// <summary>
        /// Adds an animation to the animation manager.
        /// </summary>
        /// <param name="spriteSheet">Source to pull the sprites from.</param>
        /// <param name="spriteDims">Size of each sprite on the sheet.</param>
        /// <param name="sheetDims">Number of sprites in each dimension.</param>
        /// <param name="spriteOrder">Order to cycle through the sprites.</param>
        /// <param name="cycleRate">Number of sprites per second. 0 disables animation.</param>
        /// <param name="speed">Number of space units to move per second.</param>
        /// <param name="destination">Destination Space to stop at. Triggers destinationReached event when reached.</param>
        /// <returns>ID of the animation in the manager.</returns>
        public int addAnimation(Texture2D spriteSheet, Vector2 spriteDims, Vector2 sheetDims, List<int> spriteOrder, int cycleRate, float speed, Vector2 destination)
        {
            int ret = addAnimation(spriteSheet, spriteDims, sheetDims, spriteOrder, cycleRate);
            this.translateSpeed = speed;
            if(this.translateSpeed < 0)
            {
                this.translateSpeed = Math.Abs(this.translateSpeed);
            }
            this.translateDestination = destination;
            return ret;
        }

        public void changeMovement(float speed, Vector2 destination)
        {
            this.translateSpeed = speed;
            if (this.translateSpeed < 0)
            {
                this.translateSpeed = Math.Abs(this.translateSpeed);
            }
            this.translateDestination = destination;
        }

        public void copy(AnimationManager source)
        {
            animationList = source.animationList;
            currentAnimationId = source.currentAnimationId;
            currentSpriteId = source.currentSpriteId;
            lastUpdateTime = source.lastUpdateTime;
        }

        private void Update()
        {
            //Check if there is an animation currently active.
            if(currentAnimationId != -1 && currentAnimationId < animationList.Count)
            {
                //Check if the sprite is due for an update
                float currentTime = Time.fixedTime;
                int cycleRate = animationList[currentAnimationId].Item2;
                if (cycleRate != 0 && currentTime - lastUpdateTime > 1 / (float)cycleRate)
                {
                    //Update the sprite ID
                    currentSpriteId++;
                    if(currentSpriteId >= animationList[currentAnimationId].Item1.Count)
                    {
                        //Cycle back to sprite #0
                        currentSpriteId = 0;
                        animationDone?.Invoke(CurrentAnimationId);
                    }

                    //Update the last update time
                    lastUpdateTime = currentTime;

                    //Update the sprite
                    gameObject.GetComponent<SpriteRenderer>().sprite = animationList[currentAnimationId].Item1[currentSpriteId];
                }
            }
            if(translateSpeed != 0)
            {
                float step = translateSpeed * Time.deltaTime;
                GridRPG.Space destSpace = Game.map.GetComponent<Map>().getSpace(translateDestination);
                transform.position = Vector3.MoveTowards(transform.position, destSpace.transform.position, step);
                if(transform.position == destSpace.transform.position)
                {
                    translateSpeed = 0;
                    //Debug.Log(name + ">AnimationManager: destination reached.");
                    destinationReached?.Invoke(currentAnimationId);
                }
            }
        }
    }
}
