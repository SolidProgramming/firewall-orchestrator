Alter table "changelog_object" add  foreign key ("change_type_id") references "stm_change_type" ("change_type_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("doku_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("import_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("new_obj_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "changelog_object" add  foreign key ("old_obj_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("change_type_id") references "stm_change_type" ("change_type_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("dev_id") references "device" ("dev_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("doku_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("import_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("new_rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "changelog_rule" add  foreign key ("old_rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("change_type_id") references "stm_change_type" ("change_type_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("doku_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("import_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("new_svc_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "changelog_service" add  foreign key ("old_svc_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("change_type_id") references "stm_change_type" ("change_type_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("doku_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("import_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("new_user_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "changelog_user" add  foreign key ("old_user_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "config" add  foreign key ("config_user") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "device" add  foreign key ("dev_typ_id") references "stm_dev_typ" ("dev_typ_id") on update restrict on delete cascade;
Alter table "device" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "device" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "error_log" add  foreign key ("error_id") references "error" ("error_id") on update restrict on delete cascade;
Alter table "import_changelog" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "import_control" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "import_object" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "import_rule" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "import_service" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "import_user" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "import_zone" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "management" add  foreign key ("dev_typ_id") references "stm_dev_typ" ("dev_typ_id") on update restrict on delete cascade;
Alter table "management" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("last_change_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("nattyp_id") references "stm_nattyp" ("nattyp_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("obj_color_id") references "stm_color" ("color_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("obj_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("obj_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("obj_nat_install") references "device" ("dev_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("obj_typ_id") references "stm_obj_typ" ("obj_typ_id") on update restrict on delete cascade;
Alter table "object" add  foreign key ("zone_id") references "zone" ("zone_id") on update restrict on delete cascade;
Alter table "objgrp" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "objgrp" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "objgrp" add  foreign key ("objgrp_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "objgrp" add  foreign key ("objgrp_member_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "objgrp_flat" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "objgrp_flat" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "objgrp_flat" add  foreign key ("objgrp_flat_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "objgrp_flat" add  foreign key ("objgrp_flat_member_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "report" add foreign key ("report_template_id") references "report_template" ("report_template_id") on update restrict on delete cascade;
Alter table "report" add foreign key ("start_import_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "report" add foreign key ("stop_import_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "report" add foreign key ("report_owner_id") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "report_schedule" add foreign key ("report_template_id") references "report_template" ("report_template_id") on update restrict on delete cascade;
Alter table "report_schedule" add foreign key ("report_schedule_owner") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "report_template" add foreign key ("report_typ_id") references "stm_report_typ" ("report_typ_id") on update restrict on delete cascade;
Alter table "report_template_viewable_by_tenant" add foreign key ("report_template_id") references "report_template" ("report_template_id") on update restrict on delete cascade;
Alter table "report_template_viewable_by_tenant" add foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "report_template_viewable_by_user" add foreign key ("report_template_id") references "report_template" ("report_template_id") on update restrict on delete cascade;
Alter table "report_template_viewable_by_user" add foreign key ("uiuser_id") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "request" add  foreign key ("request_type_id") references "request_type" ("request_type_id") on update restrict on delete cascade;
Alter table "request" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "request_object_change" add  foreign key ("log_obj_id") references "changelog_object" ("log_obj_id") on update restrict on delete cascade;
Alter table "request_object_change" add  foreign key ("request_id") references "request" ("request_id") on update restrict on delete cascade;
Alter table "request_rule_change" add  foreign key ("log_rule_id") references "changelog_rule" ("log_rule_id") on update restrict on delete cascade;
Alter table "request_rule_change" add  foreign key ("request_id") references "request" ("request_id") on update restrict on delete cascade;
Alter table "request_service_change" add  foreign key ("log_svc_id") references "changelog_service" ("log_svc_id") on update restrict on delete cascade;
Alter table "request_service_change" add  foreign key ("request_id") references "request" ("request_id") on update restrict on delete cascade;
Alter table "request_user_change" add  foreign key ("log_usr_id") references "changelog_user" ("log_usr_id") on update restrict on delete cascade;
Alter table "request_user_change" add  foreign key ("request_id") references "request" ("request_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("action_id") references "stm_action" ("action_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("dev_id") references "device" ("dev_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("last_change_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("rule_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("rule_from_zone") references "zone" ("zone_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("rule_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("rule_to_zone") references "zone" ("zone_id") on update restrict on delete cascade;
Alter table "rule" add  foreign key ("track_id") references "stm_track" ("track_id") on update restrict on delete cascade;
Alter table "rule_from" add  foreign key ("obj_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "rule_from" add  foreign key ("rf_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_from" add  foreign key ("rf_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_from" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "rule_from" add  foreign key ("user_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "rule_order" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_order" add  foreign key ("dev_id") references "device" ("dev_id") on update restrict on delete cascade;
Alter table "rule_order" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "rule_review" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "rule_review" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "rule_service" add  foreign key ("rs_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_service" add  foreign key ("rs_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_service" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "rule_service" add  foreign key ("svc_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "rule_to" add  foreign key ("obj_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "rule_to" add  foreign key ("rt_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_to" add  foreign key ("rt_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "rule_to" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("ip_proto_id") references "stm_ip_proto" ("ip_proto_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("last_change_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("svc_color_id") references "stm_color" ("color_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("svc_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("svc_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "service" add  foreign key ("svc_typ_id") references "stm_svc_typ" ("svc_typ_id") on update restrict on delete cascade;
Alter table "svcgrp" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "svcgrp" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "svcgrp" add  foreign key ("svcgrp_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "svcgrp" add  foreign key ("svcgrp_member_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "svcgrp_flat" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "svcgrp_flat" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "svcgrp_flat" add  foreign key ("svcgrp_flat_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "svcgrp_flat" add  foreign key ("svcgrp_flat_member_id") references "service" ("svc_id") on update restrict on delete cascade;
Alter table "temp_filtered_rule_ids" add  foreign key ("rule_id") references "rule" ("rule_id") on update restrict on delete cascade;
Alter table "temp_mgmid_importid_at_report_time" add  foreign key ("control_id") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "temp_mgmid_importid_at_report_time" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "tenant_network" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "tenant_object" add  foreign key ("obj_id") references "object" ("obj_id") on update restrict on delete cascade;
Alter table "tenant_object" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "tenant_to_device" add  foreign key ("device_id") references "device" ("dev_id") on update restrict on delete cascade;
Alter table "tenant_to_device" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "tenant_user" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "tenant_user" add  foreign key ("user_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "tenant_username" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "txt" add foreign key ("language") references "language" ("name") on update restrict on delete cascade;
Alter table "uiuser" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "uiuser" add foreign key ("uiuser_language") references "language" ("name") on update restrict on delete cascade;
Alter table "usergrp" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "usergrp" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "usergrp" add  foreign key ("usergrp_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "usergrp" add  foreign key ("usergrp_member_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "usergrp_flat" add  foreign key ("import_created") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "usergrp_flat" add  foreign key ("import_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "usergrp_flat" add  foreign key ("usergrp_flat_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "usergrp_flat" add  foreign key ("usergrp_flat_member_id") references "usr" ("user_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("last_change_admin") references "uiuser" ("uiuser_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("tenant_id") references "tenant" ("tenant_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("user_color_id") references "stm_color" ("color_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("usr_typ_id") references "stm_usr_typ" ("usr_typ_id") on update restrict on delete cascade;

Alter table "usr" add  foreign key ("user_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "usr" add  foreign key ("user_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;

Alter table "zone" add  foreign key ("mgm_id") references "management" ("mgm_id") on update restrict on delete cascade;
Alter table "zone" add  foreign key ("zone_create") references "import_control" ("control_id") on update restrict on delete cascade;
Alter table "zone" add  foreign key ("zone_last_seen") references "import_control" ("control_id") on update restrict on delete cascade;