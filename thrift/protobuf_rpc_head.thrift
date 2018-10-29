struct ProtoBufRpcHead {
    1: optional i32 version,
    2: i32 msg_type,
    3: i64(u) session_id,
    4: string function_name,
    5: optional i32 timeout_ms,
    6: optional i64(u) timestamp,
}