extends KinematicBody2D

export(Resource) var enemy_props = preload("res://resources/DefaultPlayer.tres")

var velocity = Vector2.ZERO

#calculated enemy parameters
var GRAVITY: float
var JUMP_VELOCITY: float
var MAX_VELOCITY_X: float

func _ready():
	calculate_gravity_and_velocity()

func _physics_process(delta):
	apply_gravity(delta)
	velocity = move_and_slide(velocity, Vector2.UP)

func apply_gravity(delta):
	velocity.y += GRAVITY * delta


func calculate_gravity_and_velocity():
	GRAVITY = 8 * enemy_props.JUMP_HEIGHT / pow(enemy_props.JUMP_TIME, 2)
	JUMP_VELOCITY = 4 * enemy_props.JUMP_HEIGHT / enemy_props.JUMP_TIME
	MAX_VELOCITY_X = enemy_props.JUMP_RANGE / enemy_props.JUMP_TIME
	print(GRAVITY)
	print(JUMP_VELOCITY)
	print(MAX_VELOCITY_X)
