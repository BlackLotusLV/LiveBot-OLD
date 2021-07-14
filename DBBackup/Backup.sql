--
-- PostgreSQL database dump
--

-- Dumped from database version 13.2
-- Dumped by pg_dump version 13.2

-- Started on 2021-07-14 13:02:58

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 6 (class 2615 OID 16402)
-- Name: livebot; Type: SCHEMA; Schema: -; Owner: livebot
--

CREATE SCHEMA livebot;


ALTER SCHEMA livebot OWNER TO livebot;

--
-- TOC entry 3198 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA livebot; Type: COMMENT; Schema: -; Owner: livebot
--

COMMENT ON SCHEMA livebot IS 'standard livebot schema';


--
-- TOC entry 241 (class 1255 OID 16403)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION livebot.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 242 (class 1255 OID 16404)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION livebot.add_new_user_settings(userid text) OWNER TO livebot;

--
-- TOC entry 243 (class 1255 OID 16405)
-- Name: create_welcome_settings(); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.create_welcome_settings() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
begin
	insert into 
		livebot."Server_Welcome_Settings" (server_id) values (new.id_server);
		return new;
end;
$$;


ALTER FUNCTION livebot.create_welcome_settings() OWNER TO livebot;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 229 (class 1259 OID 16864)
-- Name: AM_Banned_Words; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."AM_Banned_Words" (
    id_banned_word integer NOT NULL,
    word text NOT NULL,
    server_id numeric(20,0) NOT NULL,
    offense text NOT NULL
);


ALTER TABLE livebot."AM_Banned_Words" OWNER TO livebot;

--
-- TOC entry 228 (class 1259 OID 16862)
-- Name: AM_Banned_Words_id_banned_word_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."AM_Banned_Words_id_banned_word_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."AM_Banned_Words_id_banned_word_seq" OWNER TO livebot;

--
-- TOC entry 3199 (class 0 OID 0)
-- Dependencies: 228
-- Name: AM_Banned_Words_id_banned_word_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."AM_Banned_Words_id_banned_word_seq" OWNED BY livebot."AM_Banned_Words".id_banned_word;


--
-- TOC entry 201 (class 1259 OID 16393)
-- Name: Background_Image; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Background_Image" (
    id_bg integer NOT NULL,
    image bytea NOT NULL,
    name text NOT NULL,
    price bigint
);


ALTER TABLE livebot."Background_Image" OWNER TO livebot;

--
-- TOC entry 231 (class 1259 OID 33561)
-- Name: Bot_Output_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Bot_Output_List" (
    id_output integer NOT NULL,
    command text NOT NULL,
    language text DEFAULT 'gb'::text NOT NULL,
    command_text text NOT NULL
);


ALTER TABLE livebot."Bot_Output_List" OWNER TO livebot;

--
-- TOC entry 230 (class 1259 OID 33559)
-- Name: Bot_Output_List_id_output_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Bot_Output_List_id_output_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Bot_Output_List_id_output_seq" OWNER TO livebot;

--
-- TOC entry 3200 (class 0 OID 0)
-- Dependencies: 230
-- Name: Bot_Output_List_id_output_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Bot_Output_List_id_output_seq" OWNED BY livebot."Bot_Output_List".id_output;


--
-- TOC entry 240 (class 1259 OID 74774)
-- Name: Button_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Button_Roles" (
    id integer NOT NULL,
    button_id numeric(20,0) NOT NULL,
    server_id numeric(20,0) NOT NULL,
    channel_id numeric(20,0) NOT NULL,
    info text
);


ALTER TABLE livebot."Button_Roles" OWNER TO livebot;

--
-- TOC entry 239 (class 1259 OID 74772)
-- Name: Button_Roles_id_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Button_Roles_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Button_Roles_id_seq" OWNER TO livebot;

--
-- TOC entry 3201 (class 0 OID 0)
-- Dependencies: 239
-- Name: Button_Roles_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Button_Roles_id_seq" OWNED BY livebot."Button_Roles".id;


--
-- TOC entry 202 (class 1259 OID 16399)
-- Name: Commands_Used_Count; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Commands_Used_Count" (
    command_id integer NOT NULL,
    command text NOT NULL,
    used_count bigint DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Commands_Used_Count" OWNER TO livebot;

--
-- TOC entry 203 (class 1259 OID 16406)
-- Name: CUCDesc; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."CUCDesc" AS
 SELECT "Commands_Used_Count".command,
    "Commands_Used_Count".used_count
   FROM livebot."Commands_Used_Count"
  ORDER BY "Commands_Used_Count".used_count DESC;


ALTER TABLE livebot."CUCDesc" OWNER TO livebot;

--
-- TOC entry 204 (class 1259 OID 16410)
-- Name: Commands_Used_Count_command_id_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Commands_Used_Count_command_id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Commands_Used_Count_command_id_seq" OWNER TO livebot;

--
-- TOC entry 3202 (class 0 OID 0)
-- Dependencies: 204
-- Name: Commands_Used_Count_command_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Commands_Used_Count_command_id_seq" OWNED BY livebot."Commands_Used_Count".command_id;


--
-- TOC entry 206 (class 1259 OID 16423)
-- Name: Discipline_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Discipline_List" (
    id_discipline integer NOT NULL,
    family text NOT NULL,
    discipline_name text NOT NULL
);


ALTER TABLE livebot."Discipline_List" OWNER TO livebot;

--
-- TOC entry 205 (class 1259 OID 16412)
-- Name: Vehicle_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Vehicle_List" (
    id_vehicle integer NOT NULL,
    discipline integer NOT NULL,
    brand text NOT NULL,
    model text NOT NULL,
    year text NOT NULL,
    type text NOT NULL,
    summit_vehicle boolean DEFAULT false NOT NULL,
    selected boolean DEFAULT false NOT NULL,
    cc_only boolean DEFAULT false NOT NULL,
    tier character(1) DEFAULT '-'::bpchar NOT NULL,
    motorpass boolean DEFAULT false NOT NULL
);


ALTER TABLE livebot."Vehicle_List" OWNER TO livebot;

--
-- TOC entry 207 (class 1259 OID 16429)
-- Name: EditionVehicles; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."EditionVehicles" WITH (security_barrier='false') AS
 SELECT concat_ws(' '::text, "Vehicle_List".brand, "Vehicle_List".model) AS "Vehicle"
   FROM livebot."Vehicle_List"
  WHERE (("Vehicle_List".model ~ 'Edition'::text) AND ("Vehicle_List".model !~ 'SLR McLaren 722 Edition'::text) AND ("Vehicle_List".model !~ 'NZ Edition'::text));


ALTER TABLE livebot."EditionVehicles" OWNER TO livebot;

--
-- TOC entry 208 (class 1259 OID 16433)
-- Name: Leaderboard; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Leaderboard" (
    id_user numeric(20,0) NOT NULL,
    followers bigint DEFAULT 0 NOT NULL,
    level integer DEFAULT 0 NOT NULL,
    bucks bigint DEFAULT 0 NOT NULL,
    daily_used text,
    cookie_given integer DEFAULT 0 NOT NULL,
    cookie_taken integer DEFAULT 0 NOT NULL,
    cookie_used text,
    parent_user_id numeric(20,0)
);


ALTER TABLE livebot."Leaderboard" OWNER TO livebot;

--
-- TOC entry 235 (class 1259 OID 42085)
-- Name: Mod_Mail; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Mod_Mail" (
    id_modmail bigint NOT NULL,
    server_id numeric(20,0) NOT NULL,
    user_id numeric(20,0) NOT NULL,
    last_message_time timestamp without time zone NOT NULL,
    has_chatted boolean DEFAULT false NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    color_hex text NOT NULL
);


ALTER TABLE livebot."Mod_Mail" OWNER TO livebot;

--
-- TOC entry 234 (class 1259 OID 42083)
-- Name: Mod_Mail_id_modmail_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Mod_Mail_id_modmail_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Mod_Mail_id_modmail_seq" OWNER TO livebot;

--
-- TOC entry 3203 (class 0 OID 0)
-- Dependencies: 234
-- Name: Mod_Mail_id_modmail_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Mod_Mail_id_modmail_seq" OWNED BY livebot."Mod_Mail".id_modmail;


--
-- TOC entry 209 (class 1259 OID 16444)
-- Name: Rank_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Rank_Roles" (
    id_rank_roles integer NOT NULL,
    server_id numeric(20,0) NOT NULL,
    role_id numeric(20,0) NOT NULL,
    server_rank bigint NOT NULL
);


ALTER TABLE livebot."Rank_Roles" OWNER TO livebot;

--
-- TOC entry 210 (class 1259 OID 16450)
-- Name: Rank_Roles_id_rank_roles_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Rank_Roles_id_rank_roles_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Rank_Roles_id_rank_roles_seq" OWNER TO livebot;

--
-- TOC entry 3204 (class 0 OID 0)
-- Dependencies: 210
-- Name: Rank_Roles_id_rank_roles_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Rank_Roles_id_rank_roles_seq" OWNED BY livebot."Rank_Roles".id_rank_roles;


--
-- TOC entry 211 (class 1259 OID 16452)
-- Name: Reaction_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Reaction_Roles" (
    id integer NOT NULL,
    role_id numeric(20,0) NOT NULL,
    server_id numeric(20,0) NOT NULL,
    message_id numeric(20,0) NOT NULL,
    reaction_id numeric(20,0) NOT NULL,
    type text NOT NULL
);


ALTER TABLE livebot."Reaction_Roles" OWNER TO livebot;

--
-- TOC entry 237 (class 1259 OID 74553)
-- Name: Role_Tag_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Role_Tag_Settings" (
    id_role_tag bigint NOT NULL,
    server_id numeric(20,0) NOT NULL,
    role_id numeric(20,0) NOT NULL,
    cooldown_minutes integer DEFAULT 60 NOT NULL,
    last_used timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    emoji_id numeric(20,0) NOT NULL,
    message text,
    channel_id numeric(20,0) NOT NULL
);


ALTER TABLE livebot."Role_Tag_Settings" OWNER TO livebot;

--
-- TOC entry 212 (class 1259 OID 16458)
-- Name: Server_Ranks; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Ranks" (
    id_server_rank integer NOT NULL,
    user_id numeric(20,0) NOT NULL,
    server_id numeric(20,0) NOT NULL,
    followers bigint DEFAULT 0 NOT NULL,
    warning_level integer DEFAULT 0 NOT NULL,
    kick_count integer DEFAULT 0 NOT NULL,
    ban_count integer DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Server_Ranks" OWNER TO livebot;

--
-- TOC entry 213 (class 1259 OID 16468)
-- Name: Server_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Settings" (
    id_server numeric(20,0) NOT NULL,
    delete_log numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    user_traffic numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    wkb_log numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    spam_exception numeric(20,0)[] DEFAULT '{0}'::numeric[] NOT NULL,
    mod_mail numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    has_link_protection boolean DEFAULT false NOT NULL,
    voice_activity_log numeric(20,0) DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Server_Settings" OWNER TO livebot;

--
-- TOC entry 238 (class 1259 OID 74706)
-- Name: Server_Welcome_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Welcome_Settings" (
    server_id numeric(20,0) NOT NULL,
    channel_id numeric(20,0) DEFAULT 0 NOT NULL,
    welcome_msg text,
    goodbye_msg text,
    has_screening boolean DEFAULT false NOT NULL
);


ALTER TABLE livebot."Server_Welcome_Settings" OWNER TO livebot;

--
-- TOC entry 214 (class 1259 OID 16478)
-- Name: Stream_Notification; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Stream_Notification" (
    stream_notification_id integer NOT NULL,
    server_id numeric(20,0) NOT NULL,
    games text[],
    roles_id numeric(20,0)[],
    channel_id numeric(20,0) NOT NULL
);


ALTER TABLE livebot."Stream_Notification" OWNER TO livebot;

--
-- TOC entry 215 (class 1259 OID 16484)
-- Name: UniqueVehicles; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."UniqueVehicles" AS
 SELECT DISTINCT concat_ws(' '::text, "Vehicle_List".brand, "Vehicle_List".model) AS "Vehicle",
    count(*) OVER (PARTITION BY "Vehicle_List".model) AS "Disciplines"
   FROM livebot."Vehicle_List"
  ORDER BY (count(*) OVER (PARTITION BY "Vehicle_List".model)) DESC;


ALTER TABLE livebot."UniqueVehicles" OWNER TO livebot;

--
-- TOC entry 216 (class 1259 OID 16488)
-- Name: User_Images; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Images" (
    id_user_images integer NOT NULL,
    user_id numeric(20,0) NOT NULL,
    bg_id integer NOT NULL
);


ALTER TABLE livebot."User_Images" OWNER TO livebot;

--
-- TOC entry 217 (class 1259 OID 16494)
-- Name: User_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Settings" (
    id_user_settings integer NOT NULL,
    user_id numeric(20,0) NOT NULL,
    image_id integer NOT NULL,
    user_info text NOT NULL
);


ALTER TABLE livebot."User_Settings" OWNER TO livebot;

--
-- TOC entry 218 (class 1259 OID 16500)
-- Name: Warnings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Warnings" (
    id_warning integer NOT NULL,
    reason text NOT NULL COLLATE pg_catalog."C.UTF-8",
    active boolean NOT NULL,
    date text NOT NULL,
    admin_id numeric(20,0) NOT NULL,
    user_id numeric(20,0) NOT NULL,
    server_id numeric(20,0) NOT NULL,
    type text DEFAULT 'warning'::text NOT NULL
);


ALTER TABLE livebot."Warnings" OWNER TO livebot;

--
-- TOC entry 233 (class 1259 OID 33592)
-- Name: Weather_Schedule; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Weather_Schedule" (
    id integer NOT NULL,
    "time" time without time zone NOT NULL,
    day integer NOT NULL,
    weather text NOT NULL
);


ALTER TABLE livebot."Weather_Schedule" OWNER TO livebot;

--
-- TOC entry 232 (class 1259 OID 33590)
-- Name: Weather_Schedule_id_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."Weather_Schedule_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."Weather_Schedule_id_seq" OWNER TO livebot;

--
-- TOC entry 3205 (class 0 OID 0)
-- Dependencies: 232
-- Name: Weather_Schedule_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Weather_Schedule_id_seq" OWNED BY livebot."Weather_Schedule".id;


--
-- TOC entry 219 (class 1259 OID 16506)
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.backgrond_image_id_bg_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.backgrond_image_id_bg_seq OWNER TO livebot;

--
-- TOC entry 3206 (class 0 OID 0)
-- Dependencies: 219
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.backgrond_image_id_bg_seq OWNED BY livebot."Background_Image".id_bg;


--
-- TOC entry 220 (class 1259 OID 16508)
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.discipline_list_id_discipline_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.discipline_list_id_discipline_seq OWNER TO livebot;

--
-- TOC entry 3207 (class 0 OID 0)
-- Dependencies: 220
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.discipline_list_id_discipline_seq OWNED BY livebot."Discipline_List".id_discipline;


--
-- TOC entry 221 (class 1259 OID 16510)
-- Name: reaction_roles_id_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.reaction_roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.reaction_roles_id_seq OWNER TO livebot;

--
-- TOC entry 3208 (class 0 OID 0)
-- Dependencies: 221
-- Name: reaction_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.reaction_roles_id_seq OWNED BY livebot."Reaction_Roles".id;


--
-- TOC entry 236 (class 1259 OID 74551)
-- Name: roletagsettings_id_role_tag_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.roletagsettings_id_role_tag_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.roletagsettings_id_role_tag_seq OWNER TO livebot;

--
-- TOC entry 3209 (class 0 OID 0)
-- Dependencies: 236
-- Name: roletagsettings_id_role_tag_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.roletagsettings_id_role_tag_seq OWNED BY livebot."Role_Tag_Settings".id_role_tag;


--
-- TOC entry 222 (class 1259 OID 16512)
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot."server_ranks_Id_server_rank_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot."server_ranks_Id_server_rank_seq" OWNER TO livebot;

--
-- TOC entry 3210 (class 0 OID 0)
-- Dependencies: 222
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."server_ranks_Id_server_rank_seq" OWNED BY livebot."Server_Ranks".id_server_rank;


--
-- TOC entry 223 (class 1259 OID 16514)
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.stream_notification_stream_notification_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.stream_notification_stream_notification_id_seq OWNER TO livebot;

--
-- TOC entry 3211 (class 0 OID 0)
-- Dependencies: 223
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.stream_notification_stream_notification_id_seq OWNED BY livebot."Stream_Notification".stream_notification_id;


--
-- TOC entry 224 (class 1259 OID 16516)
-- Name: user_images_id_user_images_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.user_images_id_user_images_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.user_images_id_user_images_seq OWNER TO livebot;

--
-- TOC entry 3212 (class 0 OID 0)
-- Dependencies: 224
-- Name: user_images_id_user_images_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_images_id_user_images_seq OWNED BY livebot."User_Images".id_user_images;


--
-- TOC entry 225 (class 1259 OID 16518)
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.user_settings_id_user_settings_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.user_settings_id_user_settings_seq OWNER TO livebot;

--
-- TOC entry 3213 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_settings_id_user_settings_seq OWNED BY livebot."User_Settings".id_user_settings;


--
-- TOC entry 226 (class 1259 OID 16520)
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.vehicle_list_id_vehicle_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.vehicle_list_id_vehicle_seq OWNER TO livebot;

--
-- TOC entry 3214 (class 0 OID 0)
-- Dependencies: 226
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.vehicle_list_id_vehicle_seq OWNED BY livebot."Vehicle_List".id_vehicle;


--
-- TOC entry 227 (class 1259 OID 16522)
-- Name: warnings_id_warning_seq; Type: SEQUENCE; Schema: livebot; Owner: livebot
--

CREATE SEQUENCE livebot.warnings_id_warning_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE livebot.warnings_id_warning_seq OWNER TO livebot;

--
-- TOC entry 3215 (class 0 OID 0)
-- Dependencies: 227
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.warnings_id_warning_seq OWNED BY livebot."Warnings".id_warning;


--
-- TOC entry 2981 (class 2604 OID 16442)
-- Name: AM_Banned_Words id_banned_word; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words" ALTER COLUMN id_banned_word SET DEFAULT nextval('livebot."AM_Banned_Words_id_banned_word_seq"'::regclass);


--
-- TOC entry 2947 (class 2604 OID 16443)
-- Name: Background_Image id_bg; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image" ALTER COLUMN id_bg SET DEFAULT nextval('livebot.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2983 (class 2604 OID 16444)
-- Name: Bot_Output_List id_output; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Bot_Output_List" ALTER COLUMN id_output SET DEFAULT nextval('livebot."Bot_Output_List_id_output_seq"'::regclass);


--
-- TOC entry 2993 (class 2604 OID 74777)
-- Name: Button_Roles id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Button_Roles" ALTER COLUMN id SET DEFAULT nextval('livebot."Button_Roles_id_seq"'::regclass);


--
-- TOC entry 2949 (class 2604 OID 16445)
-- Name: Commands_Used_Count command_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count" ALTER COLUMN command_id SET DEFAULT nextval('livebot."Commands_Used_Count_command_id_seq"'::regclass);


--
-- TOC entry 2956 (class 2604 OID 16446)
-- Name: Discipline_List id_discipline; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List" ALTER COLUMN id_discipline SET DEFAULT nextval('livebot.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2987 (class 2604 OID 16447)
-- Name: Mod_Mail id_modmail; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Mod_Mail" ALTER COLUMN id_modmail SET DEFAULT nextval('livebot."Mod_Mail_id_modmail_seq"'::regclass);


--
-- TOC entry 2962 (class 2604 OID 16448)
-- Name: Rank_Roles id_rank_roles; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles" ALTER COLUMN id_rank_roles SET DEFAULT nextval('livebot."Rank_Roles_id_rank_roles_seq"'::regclass);


--
-- TOC entry 2963 (class 2604 OID 16449)
-- Name: Reaction_Roles id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles" ALTER COLUMN id SET DEFAULT nextval('livebot.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2990 (class 2604 OID 16450)
-- Name: Role_Tag_Settings id_role_tag; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Role_Tag_Settings" ALTER COLUMN id_role_tag SET DEFAULT nextval('livebot.roletagsettings_id_role_tag_seq'::regclass);


--
-- TOC entry 2968 (class 2604 OID 16451)
-- Name: Server_Ranks id_server_rank; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks" ALTER COLUMN id_server_rank SET DEFAULT nextval('livebot."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2976 (class 2604 OID 16452)
-- Name: Stream_Notification stream_notification_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification" ALTER COLUMN stream_notification_id SET DEFAULT nextval('livebot.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2977 (class 2604 OID 16453)
-- Name: User_Images id_user_images; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images" ALTER COLUMN id_user_images SET DEFAULT nextval('livebot.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2978 (class 2604 OID 16454)
-- Name: User_Settings id_user_settings; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings" ALTER COLUMN id_user_settings SET DEFAULT nextval('livebot.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2955 (class 2604 OID 16455)
-- Name: Vehicle_List id_vehicle; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List" ALTER COLUMN id_vehicle SET DEFAULT nextval('livebot.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2980 (class 2604 OID 16456)
-- Name: Warnings id_warning; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings" ALTER COLUMN id_warning SET DEFAULT nextval('livebot.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2984 (class 2604 OID 16457)
-- Name: Weather_Schedule id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule" ALTER COLUMN id SET DEFAULT nextval('livebot."Weather_Schedule_id_seq"'::regclass);


--
-- TOC entry 3025 (class 2606 OID 16458)
-- Name: AM_Banned_Words AM_Banned_Words_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words"
    ADD CONSTRAINT "AM_Banned_Words_pkey" PRIMARY KEY (id_banned_word);


--
-- TOC entry 3029 (class 2606 OID 16459)
-- Name: Bot_Output_List Bot_Output_List_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Bot_Output_List"
    ADD CONSTRAINT "Bot_Output_List_pkey" PRIMARY KEY (id_output);


--
-- TOC entry 3041 (class 2606 OID 74782)
-- Name: Button_Roles Button_Roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Button_Roles"
    ADD CONSTRAINT "Button_Roles_pkey" PRIMARY KEY (id);


--
-- TOC entry 2997 (class 2606 OID 16460)
-- Name: Commands_Used_Count Commands_Used_Count_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count"
    ADD CONSTRAINT "Commands_Used_Count_pkey" PRIMARY KEY (command_id);


--
-- TOC entry 3031 (class 2606 OID 16461)
-- Name: Weather_Schedule DayTimeUnique; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule"
    ADD CONSTRAINT "DayTimeUnique" UNIQUE ("time", day);


--
-- TOC entry 3035 (class 2606 OID 16462)
-- Name: Mod_Mail Mod_Mail_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Mod_Mail"
    ADD CONSTRAINT "Mod_Mail_pkey" PRIMARY KEY (id_modmail);


--
-- TOC entry 3007 (class 2606 OID 16463)
-- Name: Rank_Roles Rank_Roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Rank_Roles_pkey" PRIMARY KEY (id_rank_roles);


--
-- TOC entry 3011 (class 2606 OID 16464)
-- Name: Server_Ranks Server-User; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server-User" UNIQUE (server_id, user_id);


--
-- TOC entry 3015 (class 2606 OID 16465)
-- Name: Server_Settings Server_Settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Settings"
    ADD CONSTRAINT "Server_Settings_pkey" PRIMARY KEY (id_server);


--
-- TOC entry 3033 (class 2606 OID 16466)
-- Name: Weather_Schedule Weather_Schedule_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule"
    ADD CONSTRAINT "Weather_Schedule_pkey" PRIMARY KEY (id);


--
-- TOC entry 3027 (class 2606 OID 16467)
-- Name: AM_Banned_Words am_banned_words_un; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words"
    ADD CONSTRAINT am_banned_words_un UNIQUE (word, server_id);


--
-- TOC entry 2995 (class 2606 OID 16468)
-- Name: Background_Image backgrond_image_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image"
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY (id_bg);


--
-- TOC entry 3001 (class 2606 OID 16469)
-- Name: Discipline_List discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE (discipline_name);


--
-- TOC entry 3003 (class 2606 OID 16470)
-- Name: Discipline_List discipline_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY (id_discipline);


--
-- TOC entry 3005 (class 2606 OID 16471)
-- Name: Leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Leaderboard"
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY (id_user);


--
-- TOC entry 3009 (class 2606 OID 16472)
-- Name: Reaction_Roles reaction_roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 3037 (class 2606 OID 16473)
-- Name: Role_Tag_Settings roletagsettings_pk; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Role_Tag_Settings"
    ADD CONSTRAINT roletagsettings_pk PRIMARY KEY (id_role_tag);


--
-- TOC entry 3013 (class 2606 OID 16474)
-- Name: Server_Ranks server_ranks_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY (id_server_rank);


--
-- TOC entry 3039 (class 2606 OID 16475)
-- Name: Server_Welcome_Settings server_welcome_settings_pk; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Welcome_Settings"
    ADD CONSTRAINT server_welcome_settings_pk PRIMARY KEY (server_id);


--
-- TOC entry 3017 (class 2606 OID 16476)
-- Name: Stream_Notification stream_notification_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY (stream_notification_id);


--
-- TOC entry 3019 (class 2606 OID 16477)
-- Name: User_Images user_images_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_pkey PRIMARY KEY (id_user_images);


--
-- TOC entry 3021 (class 2606 OID 16478)
-- Name: User_Settings user_settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY (id_user_settings);


--
-- TOC entry 2999 (class 2606 OID 16479)
-- Name: Vehicle_List vehicle_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY (id_vehicle);


--
-- TOC entry 3023 (class 2606 OID 16480)
-- Name: Warnings warnings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT warnings_pkey PRIMARY KEY (id_warning);


--
-- TOC entry 3059 (class 2620 OID 16481)
-- Name: Server_Settings create_trigger; Type: TRIGGER; Schema: livebot; Owner: livebot
--

CREATE TRIGGER create_trigger AFTER INSERT ON livebot."Server_Settings" FOR EACH ROW EXECUTE FUNCTION livebot.create_welcome_settings();


--
-- TOC entry 3053 (class 2606 OID 16482)
-- Name: AM_Banned_Words AM_Banned_Words_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words"
    ADD CONSTRAINT "AM_Banned_Words_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 3058 (class 2606 OID 74783)
-- Name: Button_Roles Button_Roles_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Button_Roles"
    ADD CONSTRAINT "Button_Roles_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server);


--
-- TOC entry 3054 (class 2606 OID 16487)
-- Name: Mod_Mail Mod_Mail_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Mod_Mail"
    ADD CONSTRAINT "Mod_Mail_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server);


--
-- TOC entry 3055 (class 2606 OID 16492)
-- Name: Mod_Mail Mod_Mail_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Mod_Mail"
    ADD CONSTRAINT "Mod_Mail_user_id_fkey" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 3044 (class 2606 OID 16497)
-- Name: Rank_Roles Rank_Roles_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Rank_Roles_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3045 (class 2606 OID 16502)
-- Name: Reaction_Roles Reaction_Roles_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT "Reaction_Roles_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3046 (class 2606 OID 16507)
-- Name: Server_Ranks Server_Ranks_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server_Ranks_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 3047 (class 2606 OID 16512)
-- Name: Server_Ranks Server_Ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server_Ranks_user_id_fkey" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user) NOT VALID;


--
-- TOC entry 3048 (class 2606 OID 16517)
-- Name: Stream_Notification Stream_Notification_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT "Stream_Notification_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 3051 (class 2606 OID 16522)
-- Name: Warnings Warnings_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT "Warnings_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 3052 (class 2606 OID 16527)
-- Name: Warnings Warnings_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT "Warnings_user_id_fkey" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user) NOT VALID;


--
-- TOC entry 3043 (class 2606 OID 16532)
-- Name: Leaderboard leaderboard_fk; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Leaderboard"
    ADD CONSTRAINT leaderboard_fk FOREIGN KEY (parent_user_id) REFERENCES livebot."Leaderboard"(id_user) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 3056 (class 2606 OID 16537)
-- Name: Role_Tag_Settings roletagsettings_fk; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Role_Tag_Settings"
    ADD CONSTRAINT roletagsettings_fk FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE;


--
-- TOC entry 3057 (class 2606 OID 16542)
-- Name: Server_Welcome_Settings server_welcome_settings_fk; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Welcome_Settings"
    ADD CONSTRAINT server_welcome_settings_fk FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE;


--
-- TOC entry 3049 (class 2606 OID 16547)
-- Name: User_Images user_images_fk; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_fk FOREIGN KEY (bg_id) REFERENCES livebot."Background_Image"(id_bg);


--
-- TOC entry 3050 (class 2606 OID 16552)
-- Name: User_Images user_images_fk_1; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_fk_1 FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 3042 (class 2606 OID 16557)
-- Name: Vehicle_List vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY (discipline) REFERENCES livebot."Discipline_List"(id_discipline);


-- Completed on 2021-07-14 13:03:20

--
-- PostgreSQL database dump complete
--

