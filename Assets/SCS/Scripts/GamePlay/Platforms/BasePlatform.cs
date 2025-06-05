using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace SCS.Scripts.GamePlay.Platforms
{
    public abstract class BasePlatform : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        protected SpriteRenderer _spriteRenderer;
        protected Rigidbody2D _rigitbody;
        protected Collider2D _collider;
        protected float _width;
        protected float _realWidth;
        protected bool _busy = true;

        //############################################################################################
        // PROPERTIES
        //############################################################################################
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (_busy != value)
                {
                    _busy = value;
                    this.gameObject.SetActive(value);
                }
            }
        }

        public int Stage { get; set; } = 0;

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        public virtual void PrePlaceSetup(float minX, float maxX, float maxWidth)
        {}

        public virtual void PostPlaceSetup()
        {}

        public virtual bool CanTakeFullLine()
        {
            return false;
        }

        public virtual Vector2 GetPosition()
        {
            return transform.position;
        }

        public float GetWidth()
        {
            return _width;
        }

        //############################################################################################
        // PROTECTED METHODS
        //############################################################################################
        protected virtual void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigitbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _realWidth = _width = _spriteRenderer.bounds.size.x;
        }

        protected virtual void FixedUpdate()
        { }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf)
                return;

            if (collision.gameObject.CompareTag("Player") && this.gameObject.activeSelf)
                foreach (ContactPoint2D contact in collision.contacts)
                    if (contact.normal.y < -0.5f)
                    {
                        Debug.Log($"{collision.gameObject.name} landed on {name}");
                        collision.gameObject.transform.SetParent(this.transform);
                        OnPlayerLanding(collision.gameObject.GetComponent<Player>());
                        break;
                    }
        }

        protected virtual void OnCollisionExit2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf)
                return;

            if (collision.gameObject.CompareTag("Player") )
            {
                collision.gameObject.transform.SetParent(null);
                OnPlayerTakeoff(collision.gameObject.GetComponent<Player>());
            }
        }

        protected virtual void OnPlayerLanding(Player player)
        {
            player.PlatformRigitbody(_rigitbody);
        }

        protected virtual void OnPlayerTakeoff(Player player)
        {
            player.PlatformRigitbody(null);
        }
    }
}
