--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.15
-- Dumped by pg_dump version 12.0

-- Started on 2019-12-05 15:59:16

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
-- TOC entry 8 (class 2615 OID 17955)
-- Name: livebot; Type: SCHEMA; Schema: -; Owner: livebot
--

CREATE SCHEMA livebot;


ALTER SCHEMA livebot OWNER TO livebot;

--
-- TOC entry 2407 (class 0 OID 0)
-- Dependencies: 8
-- Name: SCHEMA livebot; Type: COMMENT; Schema: -; Owner: livebot
--

COMMENT ON SCHEMA livebot IS 'standard livebot schema';


--
-- TOC entry 232 (class 1255 OID 27992)
-- Name: NewVehicleInserted(integer); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot."NewVehicleInserted"(x integer) RETURNS integer
    LANGUAGE sql
    AS $$select min(selected_count) from livebot."Vehicle_List" where discipline=x$$;


ALTER FUNCTION livebot."NewVehicleInserted"(x integer) OWNER TO livebot;

--
-- TOC entry 247 (class 1255 OID 17957)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION livebot.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 249 (class 1255 OID 17958)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION livebot.add_new_user_settings(userid text) OWNER TO livebot;

--
-- TOC entry 248 (class 1255 OID 16611)
-- Name: AddUserSettings(); Type: FUNCTION; Schema: public; Owner: livebot
--

CREATE FUNCTION public."AddUserSettings"() RETURNS trigger
    LANGUAGE plpgsql
    AS $$declare
userid text := new.id_user;
begin
	insert into user_images
	(user_id,bg_id)
	values (userid,1);
	declare
	imgid integer := id_user_images from user_images where user_id=userid and bg_id=1;
	begin
		insert into user_settings
		(user_id, background_colour, text_colour, border_colour, user_info, image_id)
		values (userid,'white','black','black','Just a flesh wound', imgid);
		end;
	return new;
end;$$;


ALTER FUNCTION public."AddUserSettings"() OWNER TO livebot;

--
-- TOC entry 234 (class 1255 OID 16605)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: public; Owner: livebot
--

CREATE FUNCTION public.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION public.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 233 (class 1255 OID 16604)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: public; Owner: livebot
--

CREATE FUNCTION public.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION public.add_new_user_settings(userid text) OWNER TO livebot;

SET default_tablespace = '';

--
-- TOC entry 207 (class 1259 OID 17959)
-- Name: Background_Image; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Background_Image" (
    id_bg integer NOT NULL,
    image bytea,
    name text NOT NULL,
    price bigint
);


ALTER TABLE livebot."Background_Image" OWNER TO livebot;

--
-- TOC entry 230 (class 1259 OID 28118)
-- Name: Commands_Used_Count; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Commands_Used_Count" (
    command_id integer NOT NULL,
    command text NOT NULL,
    used_count bigint DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Commands_Used_Count" OWNER TO livebot;

--
-- TOC entry 231 (class 1259 OID 28136)
-- Name: CUCDesc; Type: VIEW; Schema: livebot; Owner: livebot
--

CREATE VIEW livebot."CUCDesc" AS
 SELECT "Commands_Used_Count".command,
    "Commands_Used_Count".used_count
   FROM livebot."Commands_Used_Count"
  ORDER BY "Commands_Used_Count".used_count DESC;


ALTER TABLE livebot."CUCDesc" OWNER TO livebot;

--
-- TOC entry 229 (class 1259 OID 28116)
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
-- TOC entry 2408 (class 0 OID 0)
-- Dependencies: 229
-- Name: Commands_Used_Count_command_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Commands_Used_Count_command_id_seq" OWNED BY livebot."Commands_Used_Count".command_id;


--
-- TOC entry 209 (class 1259 OID 17967)
-- Name: Discipline_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Discipline_List" (
    id_discipline integer NOT NULL,
    family text NOT NULL,
    discipline_name text NOT NULL
);


ALTER TABLE livebot."Discipline_List" OWNER TO livebot;

--
-- TOC entry 211 (class 1259 OID 17975)
-- Name: Leaderboard; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Leaderboard" (
    id_user text NOT NULL,
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
-- TOC entry 228 (class 1259 OID 19013)
-- Name: Rank_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Rank_Roles" (
    id_rank_roles integer NOT NULL,
    server_id text NOT NULL,
    role_id text NOT NULL,
    server_rank bigint NOT NULL
);


ALTER TABLE livebot."Rank_Roles" OWNER TO livebot;

--
-- TOC entry 227 (class 1259 OID 19011)
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
-- TOC entry 2409 (class 0 OID 0)
-- Dependencies: 227
-- Name: Rank_Roles_id_rank_roles_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."Rank_Roles_id_rank_roles_seq" OWNED BY livebot."Rank_Roles".id_rank_roles;


--
-- TOC entry 212 (class 1259 OID 17984)
-- Name: Reaction_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Reaction_Roles" (
    id integer NOT NULL,
    role_id text NOT NULL,
    server_id text NOT NULL,
    message_id text NOT NULL,
    reaction_id text NOT NULL
);


ALTER TABLE livebot."Reaction_Roles" OWNER TO livebot;

--
-- TOC entry 214 (class 1259 OID 17992)
-- Name: Server_Ranks; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Ranks" (
    id_server_rank integer NOT NULL,
    user_id text NOT NULL,
    server_id text NOT NULL,
    followers bigint DEFAULT 0 NOT NULL,
    warning_level integer DEFAULT 0 NOT NULL,
    kick_count integer DEFAULT 0 NOT NULL,
    ban_count integer DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Server_Ranks" OWNER TO livebot;

--
-- TOC entry 226 (class 1259 OID 18999)
-- Name: Server_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Settings" (
    id_server text NOT NULL,
    delete_log text DEFAULT '0'::text NOT NULL,
    user_traffic text DEFAULT '0'::text NOT NULL,
    wkb_log text DEFAULT '0'::text NOT NULL,
    welcome_cwb text[] DEFAULT '{0,0,0}'::text[] NOT NULL
);


ALTER TABLE livebot."Server_Settings" OWNER TO livebot;

--
-- TOC entry 216 (class 1259 OID 18000)
-- Name: Stream_Notification; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Stream_Notification" (
    stream_notification_id integer NOT NULL,
    server_id text NOT NULL,
    games text[],
    roles_id text[],
    channel_id text NOT NULL
);


ALTER TABLE livebot."Stream_Notification" OWNER TO livebot;

--
-- TOC entry 218 (class 1259 OID 18008)
-- Name: User_Images; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Images" (
    id_user_images integer NOT NULL,
    user_id text,
    bg_id integer
);


ALTER TABLE livebot."User_Images" OWNER TO livebot;

--
-- TOC entry 220 (class 1259 OID 18016)
-- Name: User_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Settings" (
    id_user_settings integer NOT NULL,
    user_id text,
    image_id integer,
    background_colour text,
    text_colour text,
    border_colour text,
    user_info text
);


ALTER TABLE livebot."User_Settings" OWNER TO livebot;

--
-- TOC entry 222 (class 1259 OID 18034)
-- Name: Vehicle_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Vehicle_List" (
    id_vehicle integer NOT NULL,
    discipline integer NOT NULL,
    brand text NOT NULL,
    model text NOT NULL,
    year text NOT NULL,
    type text NOT NULL,
    selected_count integer DEFAULT 0 NOT NULL
);


ALTER TABLE livebot."Vehicle_List" OWNER TO livebot;

--
-- TOC entry 224 (class 1259 OID 18042)
-- Name: Warnings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Warnings" (
    id_warning integer NOT NULL,
    reason text NOT NULL COLLATE pg_catalog."C.UTF-8",
    active boolean NOT NULL,
    date text NOT NULL,
    admin_id text NOT NULL,
    user_id text NOT NULL,
    server_id text
);


ALTER TABLE livebot."Warnings" OWNER TO livebot;

--
-- TOC entry 208 (class 1259 OID 17965)
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
-- TOC entry 2410 (class 0 OID 0)
-- Dependencies: 208
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.backgrond_image_id_bg_seq OWNED BY livebot."Background_Image".id_bg;


--
-- TOC entry 210 (class 1259 OID 17973)
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
-- TOC entry 2411 (class 0 OID 0)
-- Dependencies: 210
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.discipline_list_id_discipline_seq OWNED BY livebot."Discipline_List".id_discipline;


--
-- TOC entry 213 (class 1259 OID 17990)
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
-- TOC entry 2412 (class 0 OID 0)
-- Dependencies: 213
-- Name: reaction_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.reaction_roles_id_seq OWNED BY livebot."Reaction_Roles".id;


--
-- TOC entry 215 (class 1259 OID 17998)
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
-- TOC entry 2413 (class 0 OID 0)
-- Dependencies: 215
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."server_ranks_Id_server_rank_seq" OWNED BY livebot."Server_Ranks".id_server_rank;


--
-- TOC entry 217 (class 1259 OID 18006)
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
-- TOC entry 2414 (class 0 OID 0)
-- Dependencies: 217
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.stream_notification_stream_notification_id_seq OWNED BY livebot."Stream_Notification".stream_notification_id;


--
-- TOC entry 219 (class 1259 OID 18014)
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
-- TOC entry 2415 (class 0 OID 0)
-- Dependencies: 219
-- Name: user_images_id_user_images_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_images_id_user_images_seq OWNED BY livebot."User_Images".id_user_images;


--
-- TOC entry 221 (class 1259 OID 18022)
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
-- TOC entry 2416 (class 0 OID 0)
-- Dependencies: 221
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_settings_id_user_settings_seq OWNED BY livebot."User_Settings".id_user_settings;


--
-- TOC entry 223 (class 1259 OID 18040)
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
-- TOC entry 2417 (class 0 OID 0)
-- Dependencies: 223
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.vehicle_list_id_vehicle_seq OWNED BY livebot."Vehicle_List".id_vehicle;


--
-- TOC entry 225 (class 1259 OID 18048)
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
-- TOC entry 2418 (class 0 OID 0)
-- Dependencies: 225
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.warnings_id_warning_seq OWNED BY livebot."Warnings".id_warning;


--
-- TOC entry 206 (class 1259 OID 17946)
-- Name: DisciplineList; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public."DisciplineList" (
    "ID_Discipline" integer NOT NULL,
    "Family" text NOT NULL,
    "Discipline_Name" text NOT NULL
);


ALTER TABLE public."DisciplineList" OWNER TO livebot;

--
-- TOC entry 195 (class 1259 OID 16538)
-- Name: background_image; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.background_image (
    id_bg integer NOT NULL,
    image bytea,
    name text NOT NULL,
    price bigint
);


ALTER TABLE public.background_image OWNER TO livebot;

--
-- TOC entry 194 (class 1259 OID 16536)
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.backgrond_image_id_bg_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.backgrond_image_id_bg_seq OWNER TO livebot;

--
-- TOC entry 2419 (class 0 OID 0)
-- Dependencies: 194
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.backgrond_image_id_bg_seq OWNED BY public.background_image.id_bg;


--
-- TOC entry 201 (class 1259 OID 16617)
-- Name: discipline_list; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.discipline_list (
    id_discipline integer NOT NULL,
    family text NOT NULL,
    discipline_name text NOT NULL
);


ALTER TABLE public.discipline_list OWNER TO livebot;

--
-- TOC entry 200 (class 1259 OID 16615)
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.discipline_list_id_discipline_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.discipline_list_id_discipline_seq OWNER TO livebot;

--
-- TOC entry 2420 (class 0 OID 0)
-- Dependencies: 200
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.discipline_list_id_discipline_seq OWNED BY public.discipline_list.id_discipline;


--
-- TOC entry 189 (class 1259 OID 16497)
-- Name: leaderboard; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.leaderboard (
    id_user text NOT NULL,
    followers bigint DEFAULT 0,
    level integer DEFAULT 0,
    bucks bigint DEFAULT 0
);


ALTER TABLE public.leaderboard OWNER TO livebot;

--
-- TOC entry 191 (class 1259 OID 16508)
-- Name: reaction_roles; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.reaction_roles (
    id integer NOT NULL,
    role_id text NOT NULL,
    server_id text NOT NULL,
    message_id text NOT NULL,
    reaction_id text NOT NULL
);


ALTER TABLE public.reaction_roles OWNER TO livebot;

--
-- TOC entry 190 (class 1259 OID 16506)
-- Name: reaction_roles_id_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.reaction_roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.reaction_roles_id_seq OWNER TO livebot;

--
-- TOC entry 2421 (class 0 OID 0)
-- Dependencies: 190
-- Name: reaction_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.reaction_roles_id_seq OWNED BY public.reaction_roles.id;


--
-- TOC entry 193 (class 1259 OID 16522)
-- Name: server_ranks; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.server_ranks (
    id_server_rank integer NOT NULL,
    user_id text,
    server_id text,
    followers bigint
);


ALTER TABLE public.server_ranks OWNER TO livebot;

--
-- TOC entry 192 (class 1259 OID 16520)
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public."server_ranks_Id_server_rank_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."server_ranks_Id_server_rank_seq" OWNER TO livebot;

--
-- TOC entry 2422 (class 0 OID 0)
-- Dependencies: 192
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public."server_ranks_Id_server_rank_seq" OWNED BY public.server_ranks.id_server_rank;


--
-- TOC entry 205 (class 1259 OID 17267)
-- Name: stream_notification; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.stream_notification (
    stream_notification_id integer NOT NULL,
    server_id text NOT NULL,
    games text[],
    roles_id text[],
    channel_id text NOT NULL
);


ALTER TABLE public.stream_notification OWNER TO livebot;

--
-- TOC entry 204 (class 1259 OID 17265)
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.stream_notification_stream_notification_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.stream_notification_stream_notification_id_seq OWNER TO livebot;

--
-- TOC entry 2423 (class 0 OID 0)
-- Dependencies: 204
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.stream_notification_stream_notification_id_seq OWNED BY public.stream_notification.stream_notification_id;


--
-- TOC entry 197 (class 1259 OID 16555)
-- Name: user_images; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.user_images (
    id_user_images integer NOT NULL,
    user_id text,
    bg_id integer
);


ALTER TABLE public.user_images OWNER TO livebot;

--
-- TOC entry 196 (class 1259 OID 16553)
-- Name: user_images_id_user_images_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.user_images_id_user_images_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_images_id_user_images_seq OWNER TO livebot;

--
-- TOC entry 2424 (class 0 OID 0)
-- Dependencies: 196
-- Name: user_images_id_user_images_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.user_images_id_user_images_seq OWNED BY public.user_images.id_user_images;


--
-- TOC entry 199 (class 1259 OID 16576)
-- Name: user_settings; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.user_settings (
    id_user_settings integer NOT NULL,
    user_id text,
    image_id integer,
    background_colour text,
    text_colour text,
    border_colour text,
    user_info text
);


ALTER TABLE public.user_settings OWNER TO livebot;

--
-- TOC entry 198 (class 1259 OID 16574)
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.user_settings_id_user_settings_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.user_settings_id_user_settings_seq OWNER TO livebot;

--
-- TOC entry 2425 (class 0 OID 0)
-- Dependencies: 198
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.user_settings_id_user_settings_seq OWNED BY public.user_settings.id_user_settings;


--
-- TOC entry 186 (class 1259 OID 16402)
-- Name: user_warnings; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.user_warnings (
    warning_level integer DEFAULT 0 NOT NULL,
    warning_count integer DEFAULT 0 NOT NULL,
    kick_count integer DEFAULT 0 NOT NULL,
    ban_count integer DEFAULT 0 NOT NULL,
    id_user text NOT NULL
);


ALTER TABLE public.user_warnings OWNER TO livebot;

--
-- TOC entry 203 (class 1259 OID 16659)
-- Name: vehicle_list; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.vehicle_list (
    id_vehicle integer NOT NULL,
    discipline integer NOT NULL,
    brand text NOT NULL,
    model text NOT NULL,
    year text NOT NULL,
    type text NOT NULL
);


ALTER TABLE public.vehicle_list OWNER TO livebot;

--
-- TOC entry 202 (class 1259 OID 16657)
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.vehicle_list_id_vehicle_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.vehicle_list_id_vehicle_seq OWNER TO livebot;

--
-- TOC entry 2426 (class 0 OID 0)
-- Dependencies: 202
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.vehicle_list_id_vehicle_seq OWNED BY public.vehicle_list.id_vehicle;


--
-- TOC entry 188 (class 1259 OID 16414)
-- Name: warnings; Type: TABLE; Schema: public; Owner: livebot
--

CREATE TABLE public.warnings (
    id_warning integer NOT NULL,
    reason text NOT NULL COLLATE pg_catalog."C.UTF-8",
    active boolean NOT NULL,
    date text NOT NULL,
    admin_id text NOT NULL,
    user_id text NOT NULL
);


ALTER TABLE public.warnings OWNER TO livebot;

--
-- TOC entry 187 (class 1259 OID 16412)
-- Name: warnings_id_warning_seq; Type: SEQUENCE; Schema: public; Owner: livebot
--

CREATE SEQUENCE public.warnings_id_warning_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.warnings_id_warning_seq OWNER TO livebot;

--
-- TOC entry 2427 (class 0 OID 0)
-- Dependencies: 187
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.warnings_id_warning_seq OWNED BY public.warnings.id_warning;


--
-- TOC entry 2191 (class 2604 OID 18050)
-- Name: Background_Image id_bg; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image" ALTER COLUMN id_bg SET DEFAULT nextval('livebot.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2215 (class 2604 OID 28121)
-- Name: Commands_Used_Count command_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count" ALTER COLUMN command_id SET DEFAULT nextval('livebot."Commands_Used_Count_command_id_seq"'::regclass);


--
-- TOC entry 2192 (class 2604 OID 18051)
-- Name: Discipline_List id_discipline; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List" ALTER COLUMN id_discipline SET DEFAULT nextval('livebot.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2214 (class 2604 OID 19016)
-- Name: Rank_Roles id_rank_roles; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles" ALTER COLUMN id_rank_roles SET DEFAULT nextval('livebot."Rank_Roles_id_rank_roles_seq"'::regclass);


--
-- TOC entry 2198 (class 2604 OID 18052)
-- Name: Reaction_Roles id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles" ALTER COLUMN id SET DEFAULT nextval('livebot.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2203 (class 2604 OID 18053)
-- Name: Server_Ranks id_server_rank; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks" ALTER COLUMN id_server_rank SET DEFAULT nextval('livebot."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2204 (class 2604 OID 18054)
-- Name: Stream_Notification stream_notification_id; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification" ALTER COLUMN stream_notification_id SET DEFAULT nextval('livebot.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2205 (class 2604 OID 18055)
-- Name: User_Images id_user_images; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images" ALTER COLUMN id_user_images SET DEFAULT nextval('livebot.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2206 (class 2604 OID 18056)
-- Name: User_Settings id_user_settings; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings" ALTER COLUMN id_user_settings SET DEFAULT nextval('livebot.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2207 (class 2604 OID 18057)
-- Name: Vehicle_List id_vehicle; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List" ALTER COLUMN id_vehicle SET DEFAULT nextval('livebot.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2209 (class 2604 OID 18058)
-- Name: Warnings id_warning; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings" ALTER COLUMN id_warning SET DEFAULT nextval('livebot.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2185 (class 2604 OID 16541)
-- Name: background_image id_bg; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.background_image ALTER COLUMN id_bg SET DEFAULT nextval('public.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2188 (class 2604 OID 16620)
-- Name: discipline_list id_discipline; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list ALTER COLUMN id_discipline SET DEFAULT nextval('public.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2183 (class 2604 OID 16511)
-- Name: reaction_roles id; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.reaction_roles ALTER COLUMN id SET DEFAULT nextval('public.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2184 (class 2604 OID 16525)
-- Name: server_ranks id_server_rank; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks ALTER COLUMN id_server_rank SET DEFAULT nextval('public."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2190 (class 2604 OID 17270)
-- Name: stream_notification stream_notification_id; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.stream_notification ALTER COLUMN stream_notification_id SET DEFAULT nextval('public.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2186 (class 2604 OID 16558)
-- Name: user_images id_user_images; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images ALTER COLUMN id_user_images SET DEFAULT nextval('public.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2187 (class 2604 OID 16579)
-- Name: user_settings id_user_settings; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings ALTER COLUMN id_user_settings SET DEFAULT nextval('public.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2189 (class 2604 OID 16662)
-- Name: vehicle_list id_vehicle; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list ALTER COLUMN id_vehicle SET DEFAULT nextval('public.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2179 (class 2604 OID 16417)
-- Name: warnings id_warning; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings ALTER COLUMN id_warning SET DEFAULT nextval('public.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2270 (class 2606 OID 28127)
-- Name: Commands_Used_Count Commands_Used_Count_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Commands_Used_Count"
    ADD CONSTRAINT "Commands_Used_Count_pkey" PRIMARY KEY (command_id);


--
-- TOC entry 2268 (class 2606 OID 19021)
-- Name: Rank_Roles Rank_Roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Rank_Roles_pkey" PRIMARY KEY (id_rank_roles);


--
-- TOC entry 2266 (class 2606 OID 19010)
-- Name: Server_Settings Server_Settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Settings"
    ADD CONSTRAINT "Server_Settings_pkey" PRIMARY KEY (id_server);


--
-- TOC entry 2244 (class 2606 OID 18060)
-- Name: Background_Image backgrond_image_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image"
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY (id_bg);


--
-- TOC entry 2246 (class 2606 OID 18062)
-- Name: Discipline_List discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE (discipline_name);


--
-- TOC entry 2248 (class 2606 OID 18064)
-- Name: Discipline_List discipline_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY (id_discipline);


--
-- TOC entry 2250 (class 2606 OID 18066)
-- Name: Leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Leaderboard"
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2252 (class 2606 OID 18068)
-- Name: Reaction_Roles reaction_roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 2254 (class 2606 OID 18070)
-- Name: Server_Ranks server_ranks_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY (id_server_rank);


--
-- TOC entry 2256 (class 2606 OID 18072)
-- Name: Stream_Notification stream_notification_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY (stream_notification_id);


--
-- TOC entry 2258 (class 2606 OID 18074)
-- Name: User_Images user_images_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_pkey PRIMARY KEY (id_user_images);


--
-- TOC entry 2260 (class 2606 OID 18076)
-- Name: User_Settings user_settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY (id_user_settings);


--
-- TOC entry 2262 (class 2606 OID 18080)
-- Name: Vehicle_List vehicle_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY (id_vehicle);


--
-- TOC entry 2264 (class 2606 OID 18082)
-- Name: Warnings warnings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT warnings_pkey PRIMARY KEY (id_warning);


--
-- TOC entry 2242 (class 2606 OID 17953)
-- Name: DisciplineList DisciplineList_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public."DisciplineList"
    ADD CONSTRAINT "DisciplineList_pkey" PRIMARY KEY ("ID_Discipline");


--
-- TOC entry 2228 (class 2606 OID 16546)
-- Name: background_image backgrond_image_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.background_image
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY (id_bg);


--
-- TOC entry 2234 (class 2606 OID 16638)
-- Name: discipline_list discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE (discipline_name);


--
-- TOC entry 2236 (class 2606 OID 16625)
-- Name: discipline_list discipline_list_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY (id_discipline);


--
-- TOC entry 2222 (class 2606 OID 16504)
-- Name: leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.leaderboard
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2224 (class 2606 OID 16516)
-- Name: reaction_roles reaction_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.reaction_roles
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 2226 (class 2606 OID 16530)
-- Name: server_ranks server_ranks_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY (id_server_rank);


--
-- TOC entry 2240 (class 2606 OID 17275)
-- Name: stream_notification stream_notification_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.stream_notification
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY (stream_notification_id);


--
-- TOC entry 2230 (class 2606 OID 16563)
-- Name: user_images user_images_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT user_images_pkey PRIMARY KEY (id_user_images);


--
-- TOC entry 2232 (class 2606 OID 16584)
-- Name: user_settings user_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY (id_user_settings);


--
-- TOC entry 2218 (class 2606 OID 16448)
-- Name: user_warnings user_warnings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_warnings
    ADD CONSTRAINT user_warnings_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2238 (class 2606 OID 16667)
-- Name: vehicle_list vehicle_list_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY (id_vehicle);


--
-- TOC entry 2220 (class 2606 OID 16422)
-- Name: warnings warnings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings
    ADD CONSTRAINT warnings_pkey PRIMARY KEY (id_warning);


--
-- TOC entry 2283 (class 2620 OID 16613)
-- Name: leaderboard Add_User_Settings; Type: TRIGGER; Schema: public; Owner: livebot
--

CREATE TRIGGER "Add_User_Settings" AFTER INSERT ON public.leaderboard FOR EACH ROW EXECUTE PROCEDURE public."AddUserSettings"();


--
-- TOC entry 2282 (class 2606 OID 19022)
-- Name: Rank_Roles Server_Settings; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Rank_Roles"
    ADD CONSTRAINT "Server_Settings" FOREIGN KEY (server_id) REFERENCES livebot."Server_Settings"(id_server);


--
-- TOC entry 2278 (class 2606 OID 18084)
-- Name: User_Images background to user list; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT "background to user list" FOREIGN KEY (bg_id) REFERENCES livebot."Background_Image"(id_bg);


--
-- TOC entry 2277 (class 2606 OID 18089)
-- Name: Server_Ranks server_ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_user_id_fkey FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 2280 (class 2606 OID 18094)
-- Name: User_Settings user settings to user; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT "user settings to user" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 2279 (class 2606 OID 18099)
-- Name: User_Images user to leaderboard; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT "user to leaderboard" FOREIGN KEY (user_id) REFERENCES livebot."Leaderboard"(id_user);


--
-- TOC entry 2281 (class 2606 OID 18104)
-- Name: Vehicle_List vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY (discipline) REFERENCES livebot."Discipline_List"(id_discipline);


--
-- TOC entry 2274 (class 2606 OID 16569)
-- Name: user_images background to user list; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT "background to user list" FOREIGN KEY (bg_id) REFERENCES public.background_image(id_bg);


--
-- TOC entry 2272 (class 2606 OID 16531)
-- Name: server_ranks server_ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks
    ADD CONSTRAINT server_ranks_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2275 (class 2606 OID 16585)
-- Name: user_settings user settings to user; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings
    ADD CONSTRAINT "user settings to user" FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2273 (class 2606 OID 16564)
-- Name: user_images user to leaderboard; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT "user to leaderboard" FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2276 (class 2606 OID 16668)
-- Name: vehicle_list vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY (discipline) REFERENCES public.discipline_list(id_discipline);


--
-- TOC entry 2271 (class 2606 OID 16449)
-- Name: warnings warnings_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings
    ADD CONSTRAINT warnings_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.user_warnings(id_user);


-- Completed on 2019-12-05 15:59:34

--
-- PostgreSQL database dump complete
--

