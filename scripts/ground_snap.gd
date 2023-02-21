extends KinematicBody2D
class_name GroundSnap

var player
var velocity: Vector2 = Vector2.ZERO

func _physics_process(delta):
	player = get_parent().get_node("MC")
#	position.y = 20
	var updated_y = player.update_ground_check_y()
	if updated_y:
		position.y = updated_y
	position.x = player.position.x
	if not is_on_floor():
		velocity.y += 1000
	velocity = move_and_slide(velocity, Vector2.UP)
