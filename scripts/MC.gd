extends KinematicBody2D
class_name main_player

export(Resource) var player_props = preload("res://resources/DefaultPlayer.tres")

onready var jump_buffer_timer = $JumpBufferTimer
onready var animation_tree = $AnimationTree
onready var player_sprite = $Sprite
onready var coyote_jump_timer = $CoyoteJumpTimer

#calculated player parameters
var GRAVITY: float
var JUMP_VELOCITY: float
var MAX_VELOCITY_X: float
var ACCELERATION: float
var DECELERATION: float

#local variables
var velocity = Vector2.ZERO
var max_jumps: int = 0
var buffered_jump: bool = false
var coyote_jump: bool = false
var was_on_floor: bool
var just_left_ground:bool = false
var screen_size: Vector2
const JUMP_BUTTON = "ui_up"

func _ready():
	screen_size = get_viewport_rect().size
	calculate_gravity_and_velocity()
	jump_buffer_timer.wait_time = player_props.JUMP_BUFFER_WAIT_TIME
	animation_tree.active = true

func _physics_process(delta):
	var input = Vector2.ZERO
	input.x = Input.get_axis("ui_left", "ui_right")
	
	apply_gravity(delta)
	jump_player(delta)
	move_player(input, delta)

	was_on_floor = is_on_floor()
	
	velocity = move_and_slide(velocity, Vector2.UP)
	position.x = clamp(position.x, 0, screen_size.x)
	position.y = clamp(position.y, 0, screen_size.y)

	
	if not is_on_floor() and was_on_floor:
		coyote_jump = true
		coyote_jump_timer.start()

func jump_player(delta):
	
	if is_on_floor() or coyote_jump and velocity.y >=0:
		max_jumps = player_props.MAX_IN_AIR_JUMPS
		if Input.is_action_just_pressed(JUMP_BUTTON) or buffered_jump:
			velocity.y = -JUMP_VELOCITY
			buffered_jump = false
		animation_tree.set("parameters/air_or_ground/current", 0)
	else:
		if Input.is_action_just_pressed(JUMP_BUTTON) and max_jumps > 0:
			velocity.y = -JUMP_VELOCITY
			max_jumps -= 1
		
		if Input.is_action_just_pressed(JUMP_BUTTON):
			buffered_jump = true
			jump_buffer_timer.start()
		animation_tree.set("parameters/air_or_ground/current", 1)

func move_player(input, delta):
	if input.x != 0:
		animation_tree.set("parameters/on_ground/current", 1)
		player_sprite.flip_h = input.x < 0
		velocity.x = MAX_VELOCITY_X * input.x
#		apply_acceleration(input.x, delta)         #TODO: advanced horizontal movement
	else:
		animation_tree.set("parameters/on_ground/current", 0)
		velocity.x = 0
#		apply_friction(delta)                      #TODO: advanced horizontal movement

func apply_gravity(delta):
	velocity.y += GRAVITY * delta
#	velocity.y = min(velocity.y, player_props.MAX_FALL_VELOCITY)    #FIX_ME: find the appropriate value for this 

func apply_friction(delta):
	velocity.x = move_toward(velocity.x, 0, DECELERATION * delta)

func apply_acceleration(amount, delta):
	velocity.x = move_toward(velocity.x , MAX_VELOCITY_X * amount, ACCELERATION * delta)

func calculate_gravity_and_velocity():
	GRAVITY = 8 * player_props.JUMP_HEIGHT / pow(player_props.JUMP_TIME, 2)
	JUMP_VELOCITY = 4 * player_props.JUMP_HEIGHT / player_props.JUMP_TIME
	MAX_VELOCITY_X = player_props.JUMP_RANGE / player_props.JUMP_TIME
	#calculate acceleration and deceleration

func update_ground_check_y():
	if is_on_floor():
		return position.y
	return null

func _on_JumpBufferTimer_timeout():
	buffered_jump = false


func _on_CoyoteJumpTimer_timeout():
	coyote_jump = false
