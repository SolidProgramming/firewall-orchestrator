insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingAll', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingGraphQl', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingAutodiscoveryCP', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingAutodiscovery', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingEmail', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingExternalRequest', 'false', 0) ON CONFLICT DO NOTHING;
insert into config (config_key, config_value, config_user) VALUES ('strictCertHandlingMiddlewareClient', 'false', 0) ON CONFLICT DO NOTHING;