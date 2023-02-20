extends Area2D

var tile_coordinates: Vector2
var is_target: bool = false

func init(pos):
	tile_coordinates = pos

func _on_travelNode_body_entered(body):
	if body is main_player:
		is_target = true


func _on_travelNode_body_exited(body):
	if body is main_player:
		is_target = false
