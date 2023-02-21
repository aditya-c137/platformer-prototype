extends Area2D

var tile_coordinates: Vector2
var is_target: bool = false
var direction_to_target: Vector2 = Vector2.ZERO

func init(pos):
	tile_coordinates = pos

func _physics_process(delta):
	if direction_to_target == Vector2.DOWN:
		$Sprite.flip_v = true
		$Sprite.rotation_degrees = 0
	elif direction_to_target == Vector2.RIGHT:
		$Sprite.rotation_degrees = 90
		$Sprite.flip_v = false
	elif direction_to_target == Vector2.LEFT:
		$Sprite.rotation_degrees = 90
		$Sprite.flip_v = true
	else:
		$Sprite.rotation_degrees = 0
		$Sprite.flip_v = false

func _on_travelNode_body_entered(body):
	if body is GroundSnap:
		GlobalData.target_node = tile_coordinates
		GlobalData.set_update(true)
		is_target = true

func _on_travelNode_body_exited(body):
	if body.is_in_group("ground_checker"):
		is_target = false

func _on_travelNode_tree_entered():
	GlobalData.travel_nodes[tile_coordinates] = get_path()
