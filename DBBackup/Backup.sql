--
-- PostgreSQL database dump
--

-- Dumped from database version 12.1 (Debian 12.1-1.pgdg100+1)
-- Dumped by pg_dump version 12.2

-- Started on 2020-05-17 23:10:29

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
-- TOC entry 6 (class 2615 OID 16386)
-- Name: livebot; Type: SCHEMA; Schema: -; Owner: livebot
--

CREATE SCHEMA livebot;


ALTER SCHEMA livebot OWNER TO livebot;

--
-- TOC entry 3109 (class 0 OID 0)
-- Dependencies: 6
-- Name: SCHEMA livebot; Type: COMMENT; Schema: -; Owner: livebot
--

COMMENT ON SCHEMA livebot IS 'standard livebot schema';


--
-- TOC entry 236 (class 1255 OID 16387)
-- Name: NewVehicleInserted(integer); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot."NewVehicleInserted"(x integer) RETURNS integer
    LANGUAGE sql
    AS $$select min(selected_count) from livebot."Vehicle_List" where discipline=x$$;


ALTER FUNCTION livebot."NewVehicleInserted"(x integer) OWNER TO livebot;

--
-- TOC entry 237 (class 1255 OID 16388)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION livebot.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 238 (class 1255 OID 16389)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION livebot.add_new_user_settings(userid text) OWNER TO livebot;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 231 (class 1259 OID 16864)
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
-- TOC entry 230 (class 1259 OID 16862)
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
-- TOC entry 3110 (class 0 OID 0)
-- Dependencies: 230
-- Name: AM_Banned_Words_id_banned_word_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."AM_Banned_Words_id_banned_word_seq" OWNED BY livebot."AM_Banned_Words".id_banned_word;


--
-- TOC entry 203 (class 1259 OID 16393)
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
-- TOC entry 233 (class 1259 OID 33561)
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
-- TOC entry 232 (class 1259 OID 33559)
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
-- TOC entry 3111 (class 0 OID 0)
-- Dependencies: 232
-- Name: Bot_Output_List_id_output_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Bot_Output_List_id_output_seq" OWNED BY livebot."Bot_Output_List".id_output;


--
-- TOC entry 204 (class 1259 OID 16399)
-- Name: Commands_Used_Count; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Commands_Used_Count" (
    command_id integer NOT NULL,
    command text NOT NULL,
    used_count bigint DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Commands_Used_Count" OWNER TO livebot;

--
-- TOC entry 205 (class 1259 OID 16406)
-- Name: CUCDesc; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."CUCDesc" AS
 SELECT "Commands_Used_Count".command,
    "Commands_Used_Count".used_count
   FROM livebot."Commands_Used_Count"
  ORDER BY "Commands_Used_Count".used_count DESC;


ALTER TABLE livebot."CUCDesc" OWNER TO livebot;

--
-- TOC entry 206 (class 1259 OID 16410)
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
-- TOC entry 3112 (class 0 OID 0)
-- Dependencies: 206
-- Name: Commands_Used_Count_command_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Commands_Used_Count_command_id_seq" OWNED BY livebot."Commands_Used_Count".command_id;


--
-- TOC entry 208 (class 1259 OID 16423)
-- Name: Discipline_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Discipline_List" (
    id_discipline integer NOT NULL,
    family text NOT NULL,
    discipline_name text NOT NULL
);


ALTER TABLE livebot."Discipline_List" OWNER TO livebot;

--
-- TOC entry 207 (class 1259 OID 16412)
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
    selected boolean DEFAULT false NOT NULL
);


ALTER TABLE livebot."Vehicle_List" OWNER TO livebot;

--
-- TOC entry 209 (class 1259 OID 16429)
-- Name: EditionVehicles; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."EditionVehicles" WITH (security_barrier='false') AS
 SELECT concat_ws(' '::text, "Vehicle_List".brand, "Vehicle_List".model) AS "Vehicle"
   FROM livebot."Vehicle_List"
  WHERE (("Vehicle_List".model ~ 'Edition'::text) AND ("Vehicle_List".model !~ 'SLR McLaren 722 Edition'::text) AND ("Vehicle_List".model !~ 'NZ Edition'::text));


ALTER TABLE livebot."EditionVehicles" OWNER TO livebot;

--
-- TOC entry 210 (class 1259 OID 16433)
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
    cookie_used text
);


ALTER TABLE livebot."Leaderboard" OWNER TO livebot;

--
-- TOC entry 211 (class 1259 OID 16444)
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
-- TOC entry 212 (class 1259 OID 16450)
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
-- TOC entry 3113 (class 0 OID 0)
-- Dependencies: 212
-- Name: Rank_Roles_id_rank_roles_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Rank_Roles_id_rank_roles_seq" OWNED BY livebot."Rank_Roles".id_rank_roles;


--
-- TOC entry 213 (class 1259 OID 16452)
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
-- TOC entry 214 (class 1259 OID 16458)
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
-- TOC entry 215 (class 1259 OID 16468)
-- Name: Server_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Settings" (
    id_server numeric(20,0) NOT NULL,
    delete_log numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    user_traffic numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    wkb_log numeric(20,0) DEFAULT (0)::numeric NOT NULL,
    welcome_cwb text[] DEFAULT '{0,0,0}'::text[] NOT NULL,
    spam_exception numeric(20,0)[] NOT NULL
);


ALTER TABLE livebot."Server_Settings" OWNER TO livebot;

--
-- TOC entry 216 (class 1259 OID 16478)
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
-- TOC entry 217 (class 1259 OID 16484)
-- Name: UniqueVehicles; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."UniqueVehicles" AS
 SELECT DISTINCT concat_ws(' '::text, "Vehicle_List".brand, "Vehicle_List".model) AS "Vehicle",
    count(*) OVER (PARTITION BY "Vehicle_List".model) AS "Disciplines"
   FROM livebot."Vehicle_List"
  ORDER BY (count(*) OVER (PARTITION BY "Vehicle_List".model)) DESC;


ALTER TABLE livebot."UniqueVehicles" OWNER TO livebot;

--
-- TOC entry 218 (class 1259 OID 16488)
-- Name: User_Images; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Images" (
    id_user_images integer NOT NULL,
    user_id numeric(20,0) NOT NULL,
    bg_id integer NOT NULL
);


ALTER TABLE livebot."User_Images" OWNER TO livebot;

--
-- TOC entry 219 (class 1259 OID 16494)
-- Name: User_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Settings" (
    id_user_settings integer NOT NULL,
    user_id numeric(20,0) NOT NULL,
    image_id integer NOT NULL,
    background_colour text NOT NULL,
    text_colour text NOT NULL,
    border_colour text NOT NULL,
    user_info text NOT NULL
);


ALTER TABLE livebot."User_Settings" OWNER TO livebot;

--
-- TOC entry 220 (class 1259 OID 16500)
-- Name: Warnings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Warnings" (
    id_warning integer NOT NULL,
    reason text NOT NULL COLLATE pg_catalog."C.UTF-8",
    active boolean NOT NULL,
    date text NOT NULL,
    admin_id numeric(20,0) NOT NULL,
    user_id numeric(20,0) NOT NULL,
    server_id numeric(20,0) NOT NULL
);


ALTER TABLE livebot."Warnings" OWNER TO livebot;

--
-- TOC entry 235 (class 1259 OID 33592)
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
-- TOC entry 234 (class 1259 OID 33590)
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
-- TOC entry 3114 (class 0 OID 0)
-- Dependencies: 234
-- Name: Weather_Schedule_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Weather_Schedule_id_seq" OWNED BY livebot."Weather_Schedule".id;


--
-- TOC entry 221 (class 1259 OID 16506)
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
-- TOC entry 3115 (class 0 OID 0)
-- Dependencies: 221
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.backgrond_image_id_bg_seq OWNED BY livebot."Background_Image".id_bg;


--
-- TOC entry 222 (class 1259 OID 16508)
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
-- TOC entry 3116 (class 0 OID 0)
-- Dependencies: 222
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.discipline_list_id_discipline_seq OWNED BY livebot."Discipline_List".id_discipline;


--
-- TOC entry 223 (class 1259 OID 16510)
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
-- TOC entry 3117 (class 0 OID 0)
-- Dependencies: 223
-- Name: reaction_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.reaction_roles_id_seq OWNED BY livebot."Reaction_Roles".id;


--
-- TOC entry 224 (class 1259 OID 16512)
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
-- TOC entry 3118 (class 0 OID 0)
-- Dependencies: 224
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."server_ranks_Id_server_rank_seq" OWNED BY livebot."Server_Ranks".id_server_rank;


--
-- TOC entry 225 (class 1259 OID 16514)
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
-- TOC entry 3119 (class 0 OID 0)
-- Dependencies: 225
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.stream_notification_stream_notification_id_seq OWNED BY livebot."Stream_Notification".stream_notification_id;


--
-- TOC entry 226 (class 1259 OID 16516)
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
-- TOC entry 3120 (class 0 OID 0)
-- Dependencies: 226
-- Name: user_images_id_user_images_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_images_id_user_images_seq OWNED BY livebot."User_Images".id_user_images;


--
-- TOC entry 227 (class 1259 OID 16518)
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
-- TOC entry 3121 (class 0 OID 0)
-- Dependencies: 227
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_settings_id_user_settings_seq OWNED BY livebot."User_Settings".id_user_settings;


--
-- TOC entry 228 (class 1259 OID 16520)
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
-- TOC entry 3122 (class 0 OID 0)
-- Dependencies: 228
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.vehicle_list_id_vehicle_seq OWNED BY livebot."Vehicle_List".id_vehicle;


--
-- TOC entry 229 (class 1259 OID 16522)
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
-- TOC entry 3123 (class 0 OID 0)
-- Dependencies: 229
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.warnings_id_warning_seq OWNED BY livebot."Warnings".id_warning;


--
-- TOC entry 2922 (class 2604 OID 16867)
-- Name: AM_Banned_Words id_banned_word; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words" ALTER COLUMN id_banned_word SET DEFAULT nextval('livebot."AM_Banned_Words_id_banned_word_seq"'::regclass);


--
-- TOC entry 2895 (class 2604 OID 16621)
-- Name: Background_Image id_bg; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image" ALTER COLUMN id_bg SET DEFAULT nextval('livebot.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2923 (class 2604 OID 33564)
-- Name: Bot_Output_List id_output; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Bot_Output_List" ALTER COLUMN id_output SET DEFAULT nextval('livebot."Bot_Output_List_id_output_seq"'::regclass);


--
-- TOC entry 2897 (class 2604 OID 16622)
-- Name: Commands_Used_Count command_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count" ALTER COLUMN command_id SET DEFAULT nextval('livebot."Commands_Used_Count_command_id_seq"'::regclass);


--
-- TOC entry 2901 (class 2604 OID 16623)
-- Name: Discipline_List id_discipline; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List" ALTER COLUMN id_discipline SET DEFAULT nextval('livebot.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2907 (class 2604 OID 16624)
-- Name: Rank_Roles id_rank_roles; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles" ALTER COLUMN id_rank_roles SET DEFAULT nextval('livebot."Rank_Roles_id_rank_roles_seq"'::regclass);


--
-- TOC entry 2908 (class 2604 OID 16625)
-- Name: Reaction_Roles id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles" ALTER COLUMN id SET DEFAULT nextval('livebot.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2913 (class 2604 OID 16626)
-- Name: Server_Ranks id_server_rank; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks" ALTER COLUMN id_server_rank SET DEFAULT nextval('livebot."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2918 (class 2604 OID 16627)
-- Name: Stream_Notification stream_notification_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification" ALTER COLUMN stream_notification_id SET DEFAULT nextval('livebot.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2919 (class 2604 OID 16628)
-- Name: User_Images id_user_images; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images" ALTER COLUMN id_user_images SET DEFAULT nextval('livebot.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2920 (class 2604 OID 16629)
-- Name: User_Settings id_user_settings; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings" ALTER COLUMN id_user_settings SET DEFAULT nextval('livebot.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2898 (class 2604 OID 16630)
-- Name: Vehicle_List id_vehicle; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List" ALTER COLUMN id_vehicle SET DEFAULT nextval('livebot.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2921 (class 2604 OID 16631)
-- Name: Warnings id_warning; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings" ALTER COLUMN id_warning SET DEFAULT nextval('livebot.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2925 (class 2604 OID 33595)
-- Name: Weather_Schedule id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule" ALTER COLUMN id SET DEFAULT nextval('livebot."Weather_Schedule_id_seq"'::regclass);


--
-- TOC entry 2957 (class 2606 OID 16872)
-- Name: AM_Banned_Words AM_Banned_Words_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words"
    ADD CONSTRAINT "AM_Banned_Words_pkey" PRIMARY KEY (id_banned_word);


--
-- TOC entry 2959 (class 2606 OID 33570)
-- Name: Bot_Output_List Bot_Output_List_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Bot_Output_List"
    ADD CONSTRAINT "Bot_Output_List_pkey" PRIMARY KEY (id_output);


--
-- TOC entry 2929 (class 2606 OID 16703)
-- Name: Commands_Used_Count Commands_Used_Count_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count"
    ADD CONSTRAINT "Commands_Used_Count_pkey" PRIMARY KEY (command_id);


--
-- TOC entry 2961 (class 2606 OID 33604)
-- Name: Weather_Schedule DayTimeUnique; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule"
    ADD CONSTRAINT "DayTimeUnique" UNIQUE ("time", day);


--
-- TOC entry 2939 (class 2606 OID 16705)
-- Name: Rank_Roles Rank_Roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Rank_Roles_pkey" PRIMARY KEY (id_rank_roles);


--
-- TOC entry 2943 (class 2606 OID 41863)
-- Name: Server_Ranks Server-User; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server-User" UNIQUE (server_id, user_id);


--
-- TOC entry 2947 (class 2606 OID 41823)
-- Name: Server_Settings Server_Settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Settings"
    ADD CONSTRAINT "Server_Settings_pkey" PRIMARY KEY (id_server);


--
-- TOC entry 2963 (class 2606 OID 33600)
-- Name: Weather_Schedule Weather_Schedule_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Weather_Schedule"
    ADD CONSTRAINT "Weather_Schedule_pkey" PRIMARY KEY (id);


--
-- TOC entry 2927 (class 2606 OID 16709)
-- Name: Background_Image backgrond_image_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image"
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY (id_bg);


--
-- TOC entry 2933 (class 2606 OID 16711)
-- Name: Discipline_List discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE (discipline_name);


--
-- TOC entry 2935 (class 2606 OID 16713)
-- Name: Discipline_List discipline_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY (id_discipline);


--
-- TOC entry 2937 (class 2606 OID 41881)
-- Name: Leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Leaderboard"
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2941 (class 2606 OID 16717)
-- Name: Reaction_Roles reaction_roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 2945 (class 2606 OID 16719)
-- Name: Server_Ranks server_ranks_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY (id_server_rank);


--
-- TOC entry 2949 (class 2606 OID 16721)
-- Name: Stream_Notification stream_notification_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY (stream_notification_id);


--
-- TOC entry 2951 (class 2606 OID 16723)
-- Name: User_Images user_images_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_pkey PRIMARY KEY (id_user_images);


--
-- TOC entry 2953 (class 2606 OID 16725)
-- Name: User_Settings user_settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY (id_user_settings);


--
-- TOC entry 2931 (class 2606 OID 16727)
-- Name: Vehicle_List vehicle_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY (id_vehicle);


--
-- TOC entry 2955 (class 2606 OID 16729)
-- Name: Warnings warnings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT warnings_pkey PRIMARY KEY (id_warning);


--
-- TOC entry 2974 (class 2606 OID 41907)
-- Name: AM_Banned_Words AM_Banned_Words_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."AM_Banned_Words"
    ADD CONSTRAINT "AM_Banned_Words_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 2965 (class 2606 OID 41922)
-- Name: Rank_Roles Rank_Roles_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Rank_Roles_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 2966 (class 2606 OID 41917)
-- Name: Reaction_Roles Reaction_Roles_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT "Reaction_Roles_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 2968 (class 2606 OID 41932)
-- Name: Server_Ranks Server_Ranks_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server_Ranks_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 2967 (class 2606 OID 41927)
-- Name: Server_Ranks Server_Ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT "Server_Ranks_user_id_fkey" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user) NOT VALID;


--
-- TOC entry 2969 (class 2606 OID 41937)
-- Name: Stream_Notification Stream_Notification_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT "Stream_Notification_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) ON DELETE CASCADE NOT VALID;


--
-- TOC entry 2973 (class 2606 OID 41947)
-- Name: Warnings Warnings_server_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT "Warnings_server_id_fkey" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server) NOT VALID;


--
-- TOC entry 2972 (class 2606 OID 41942)
-- Name: Warnings Warnings_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT "Warnings_user_id_fkey" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user) NOT VALID;


--
-- TOC entry 2970 (class 2606 OID 41897)
-- Name: User_Images user_images_fk; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_fk FOREIGN KEY (bg_id) REFERENCES livebot."Background_Image"(id_bg);


--
-- TOC entry 2971 (class 2606 OID 41902)
-- Name: User_Images user_images_fk_1; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_fk_1 FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 2964 (class 2606 OID 16782)
-- Name: Vehicle_List vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY (discipline) REFERENCES livebot."Discipline_List"(id_discipline);


-- Completed on 2020-05-17 23:10:49

--
-- PostgreSQL database dump complete
--

