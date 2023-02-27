using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asteroids.Model
{
    public class Nlo : Enemy
    {
        private const float MinDistance = .001f;
        private static List<Nlo> Nlos { get; } = new List<Nlo>();

        private readonly float _speed;
        public int Team { get; }

        private static List<Nlo> GetOppositeTeam(int team) => Nlos.Where(nlo => nlo.Team != team).ToList();

        public Nlo(Vector2 position, float speed, int team) : base(position, 0)
        {
            Team = team;
            _speed = speed;
            Nlos.Add(this);
            Destroying += () => Nlos.Remove(this);
        }

        public override void Update(float deltaTime)
        {
            var oppositeTeam = GetOppositeTeam(Team);
            if (!oppositeTeam.Any())
                return;

            var target = oppositeTeam.Aggregate((targetA, targetB) =>
                Vector2.Distance(Position, targetB.Position) < Vector2.Distance(Position, targetA.Position)
                    ? targetB
                    : targetA);

            if (Vector2.Distance(target.Position, Position) < MinDistance)
            {
                Destroy();
                target.Destroy();
                return;
            }

            Vector2 nextPosition = Vector2.MoveTowards(Position, target.Position, _speed * deltaTime);
            MoveTo(nextPosition);
            LookAt(target.Position);
        }

        private void LookAt(Vector2 point)
        {
            Rotate(Vector2.SignedAngle(Quaternion.Euler(0, 0, Rotation) * Vector3.up, (Position - point)));
        }
    }
}