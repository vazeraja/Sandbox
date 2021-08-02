using UnityEngine;

public class CollisionDetection : MonoBehaviour {
    [HideInInspector] public new Rigidbody2D rigidbody2D;
    private ContactFilter2D contactFilter;
    public ContactPoint2D? groundContact;
    public ContactPoint2D? ceilingContact;
    public ContactPoint2D? wallContact;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[16];

    private float maxWalkCos = 0.2f;
    [HideInInspector] public int enemyLayer;

    public bool IsGrounded => groundContact.HasValue;
    public bool IsTouchingWall => wallContact.HasValue;
    public bool IsTouchingCeiling => ceilingContact.HasValue;

    public Vector2 Velocity => rigidbody2D.velocity;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        enemyLayer = LayerMask.NameToLayer($"Enemy");
    }

    private void FixedUpdate() {
        FindContacts();
    }

    private void FindContacts() {
        groundContact = null;
        ceilingContact = null;
        wallContact = null;

        float groundProjection = maxWalkCos;
        float wallProjection = maxWalkCos;
        float ceilingProjection = -maxWalkCos;

        int numberOfContacts = rigidbody2D.GetContacts(contactFilter, contacts);
        for (var i = 0; i < numberOfContacts; i++) {
            var contact = contacts[i];
            float projection = Vector2.Dot(Vector2.up, contact.normal);

            if (projection > groundProjection) {
                groundContact = contact;
                groundProjection = projection;
            }
            else if (projection < ceilingProjection) {
                ceilingContact = contact;
                ceilingProjection = projection;
            }
            else if (projection <= wallProjection) {
                wallContact = contact;
                wallProjection = projection;
            }
        }
    }
}