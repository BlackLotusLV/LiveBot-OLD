--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.13
-- Dumped by pg_dump version 11.2

-- Started on 2019-06-10 03:30:39

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 8 (class 2615 OID 17955)
-- Name: livebot; Type: SCHEMA; Schema: -; Owner: livebot
--

CREATE SCHEMA livebot;


ALTER SCHEMA livebot OWNER TO livebot;

--
-- TOC entry 2345 (class 0 OID 0)
-- Dependencies: 8
-- Name: SCHEMA livebot; Type: COMMENT; Schema: -; Owner: livebot
--

COMMENT ON SCHEMA livebot IS 'standard livebot schema';


--
-- TOC entry 240 (class 1255 OID 17956)
-- Name: AddUserSettings(); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot."AddUserSettings"() RETURNS trigger
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


ALTER FUNCTION livebot."AddUserSettings"() OWNER TO livebot;

--
-- TOC entry 253 (class 1255 OID 17957)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION livebot.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 255 (class 1255 OID 17958)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: livebot; Owner: livebot
--

CREATE FUNCTION livebot.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION livebot.add_new_user_settings(userid text) OWNER TO livebot;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 218 (class 1259 OID 17959)
-- Name: Background_Image; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Background_Image" (
    "ID_BG" integer NOT NULL,
    "Image" bytea,
    "Name" text NOT NULL,
    "Price" bigint
);


ALTER TABLE livebot."Background_Image" OWNER TO livebot;

--
-- TOC entry 220 (class 1259 OID 17967)
-- Name: Discipline_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Discipline_List" (
    "ID_Discipline" integer NOT NULL,
    "Family" text NOT NULL,
    "Discipline_Name" text NOT NULL
);


ALTER TABLE livebot."Discipline_List" OWNER TO livebot;

--
-- TOC entry 222 (class 1259 OID 17975)
-- Name: Leaderboard; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Leaderboard" (
    "ID_User" text NOT NULL,
    "Followers" bigint DEFAULT 0,
    "Level" integer DEFAULT 0,
    "Bucks" bigint DEFAULT 0
);


ALTER TABLE livebot."Leaderboard" OWNER TO livebot;

--
-- TOC entry 223 (class 1259 OID 17984)
-- Name: Reaction_Roles; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Reaction_Roles" (
    "ID" integer NOT NULL,
    "Role_ID" text NOT NULL,
    "Server_ID" text NOT NULL,
    "Message_ID" text NOT NULL,
    "Reaction_ID" text NOT NULL
);


ALTER TABLE livebot."Reaction_Roles" OWNER TO livebot;

--
-- TOC entry 225 (class 1259 OID 17992)
-- Name: Server_Ranks; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Server_Ranks" (
    "ID_Server_Rank" integer NOT NULL,
    "User_ID" text,
    "Server_ID" text,
    "Followers" bigint
);


ALTER TABLE livebot."Server_Ranks" OWNER TO livebot;

--
-- TOC entry 227 (class 1259 OID 18000)
-- Name: Stream_Notification; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Stream_Notification" (
    "Stream_Notification_ID" integer NOT NULL,
    "Server_ID" text NOT NULL,
    "Games" text[],
    "Roles_ID" text[],
    "Channel_ID" text NOT NULL
);


ALTER TABLE livebot."Stream_Notification" OWNER TO livebot;

--
-- TOC entry 229 (class 1259 OID 18008)
-- Name: User_Images; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Images" (
    "ID_User_Images" integer NOT NULL,
    "User_ID" text,
    "BG_ID" integer
);


ALTER TABLE livebot."User_Images" OWNER TO livebot;

--
-- TOC entry 231 (class 1259 OID 18016)
-- Name: User_Settings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Settings" (
    "ID_User_Settings" integer NOT NULL,
    "User_ID" text,
    "Image_ID" integer,
    "Background_Colour" text,
    "Text_Colour" text,
    "Border_Colour" text,
    "User_Info" text
);


ALTER TABLE livebot."User_Settings" OWNER TO livebot;

--
-- TOC entry 233 (class 1259 OID 18024)
-- Name: User_Warnings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."User_Warnings" (
    "Warning_Level" integer DEFAULT 0 NOT NULL,
    "Warning_Count" integer DEFAULT 0 NOT NULL,
    "Kick_Count" integer DEFAULT 0 NOT NULL,
    "Ban_Count" integer DEFAULT 0 NOT NULL,
    "ID_User" text NOT NULL
);


ALTER TABLE livebot."User_Warnings" OWNER TO livebot;

--
-- TOC entry 234 (class 1259 OID 18034)
-- Name: Vehicle_List; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Vehicle_List" (
    "ID_Vehicle" integer NOT NULL,
    "Discipline" integer NOT NULL,
    "Brand" text NOT NULL,
    "Model" text NOT NULL,
    "Year" text NOT NULL,
    "Type" text NOT NULL
);


ALTER TABLE livebot."Vehicle_List" OWNER TO livebot;

--
-- TOC entry 236 (class 1259 OID 18042)
-- Name: Warnings; Type: TABLE; Schema: livebot; Owner: livebot
--

CREATE TABLE livebot."Warnings" (
    "ID_Warning" integer NOT NULL,
    "Reason" text COLLATE pg_catalog."C.UTF-8" NOT NULL,
    "Active" boolean NOT NULL,
    "Date" text NOT NULL,
    "Admin_ID" text NOT NULL,
    "User_ID" text NOT NULL
);


ALTER TABLE livebot."Warnings" OWNER TO livebot;

--
-- TOC entry 219 (class 1259 OID 17965)
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
-- TOC entry 2346 (class 0 OID 0)
-- Dependencies: 219
-- Name: backgrond_image_id_bg_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.backgrond_image_id_bg_seq OWNED BY livebot."Background_Image"."ID_BG";


--
-- TOC entry 221 (class 1259 OID 17973)
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
-- TOC entry 2347 (class 0 OID 0)
-- Dependencies: 221
-- Name: discipline_list_id_discipline_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.discipline_list_id_discipline_seq OWNED BY livebot."Discipline_List"."ID_Discipline";


--
-- TOC entry 224 (class 1259 OID 17990)
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
-- TOC entry 2348 (class 0 OID 0)
-- Dependencies: 224
-- Name: reaction_roles_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.reaction_roles_id_seq OWNED BY livebot."Reaction_Roles"."ID";


--
-- TOC entry 226 (class 1259 OID 17998)
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
-- TOC entry 2349 (class 0 OID 0)
-- Dependencies: 226
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot."server_ranks_Id_server_rank_seq" OWNED BY livebot."Server_Ranks"."ID_Server_Rank";


--
-- TOC entry 228 (class 1259 OID 18006)
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
-- TOC entry 2350 (class 0 OID 0)
-- Dependencies: 228
-- Name: stream_notification_stream_notification_id_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.stream_notification_stream_notification_id_seq OWNED BY livebot."Stream_Notification"."Stream_Notification_ID";


--
-- TOC entry 230 (class 1259 OID 18014)
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
-- TOC entry 2351 (class 0 OID 0)
-- Dependencies: 230
-- Name: user_images_id_user_images_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_images_id_user_images_seq OWNED BY livebot."User_Images"."ID_User_Images";


--
-- TOC entry 232 (class 1259 OID 18022)
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
-- TOC entry 2352 (class 0 OID 0)
-- Dependencies: 232
-- Name: user_settings_id_user_settings_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.user_settings_id_user_settings_seq OWNED BY livebot."User_Settings"."ID_User_Settings";


--
-- TOC entry 235 (class 1259 OID 18040)
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
-- TOC entry 2353 (class 0 OID 0)
-- Dependencies: 235
-- Name: vehicle_list_id_vehicle_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.vehicle_list_id_vehicle_seq OWNED BY livebot."Vehicle_List"."ID_Vehicle";


--
-- TOC entry 237 (class 1259 OID 18048)
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
-- TOC entry 2354 (class 0 OID 0)
-- Dependencies: 237
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: livebot; Owner: livebot
--

ALTER SEQUENCE livebot.warnings_id_warning_seq OWNED BY livebot."Warnings"."ID_Warning";


--
-- TOC entry 2176 (class 2604 OID 18050)
-- Name: Background_Image ID_BG; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image" ALTER COLUMN "ID_BG" SET DEFAULT nextval('livebot.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2177 (class 2604 OID 18051)
-- Name: Discipline_List ID_Discipline; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List" ALTER COLUMN "ID_Discipline" SET DEFAULT nextval('livebot.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2181 (class 2604 OID 18052)
-- Name: Reaction_Roles ID; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles" ALTER COLUMN "ID" SET DEFAULT nextval('livebot.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2182 (class 2604 OID 18053)
-- Name: Server_Ranks ID_Server_Rank; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks" ALTER COLUMN "ID_Server_Rank" SET DEFAULT nextval('livebot."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2183 (class 2604 OID 18054)
-- Name: Stream_Notification Stream_Notification_ID; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification" ALTER COLUMN "Stream_Notification_ID" SET DEFAULT nextval('livebot.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2184 (class 2604 OID 18055)
-- Name: User_Images ID_User_Images; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images" ALTER COLUMN "ID_User_Images" SET DEFAULT nextval('livebot.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2185 (class 2604 OID 18056)
-- Name: User_Settings ID_User_Settings; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings" ALTER COLUMN "ID_User_Settings" SET DEFAULT nextval('livebot.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2190 (class 2604 OID 18057)
-- Name: Vehicle_List ID_Vehicle; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List" ALTER COLUMN "ID_Vehicle" SET DEFAULT nextval('livebot.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2191 (class 2604 OID 18058)
-- Name: Warnings ID_Warning; Type: DEFAULT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings" ALTER COLUMN "ID_Warning" SET DEFAULT nextval('livebot.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2193 (class 2606 OID 18060)
-- Name: Background_Image backgrond_image_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Background_Image"
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY ("ID_BG");


--
-- TOC entry 2195 (class 2606 OID 18062)
-- Name: Discipline_List discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE ("Discipline_Name");


--
-- TOC entry 2197 (class 2606 OID 18064)
-- Name: Discipline_List discipline_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Discipline_List"
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY ("ID_Discipline");


--
-- TOC entry 2199 (class 2606 OID 18066)
-- Name: Leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Leaderboard"
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY ("ID_User");


--
-- TOC entry 2201 (class 2606 OID 18068)
-- Name: Reaction_Roles reaction_roles_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Reaction_Roles"
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY ("ID");


--
-- TOC entry 2203 (class 2606 OID 18070)
-- Name: Server_Ranks server_ranks_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY ("ID_Server_Rank");


--
-- TOC entry 2205 (class 2606 OID 18072)
-- Name: Stream_Notification stream_notification_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Stream_Notification"
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY ("Stream_Notification_ID");


--
-- TOC entry 2207 (class 2606 OID 18074)
-- Name: User_Images user_images_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT user_images_pkey PRIMARY KEY ("ID_User_Images");


--
-- TOC entry 2209 (class 2606 OID 18076)
-- Name: User_Settings user_settings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY ("ID_User_Settings");


--
-- TOC entry 2211 (class 2606 OID 18078)
-- Name: User_Warnings user_warnings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Warnings"
    ADD CONSTRAINT user_warnings_pkey PRIMARY KEY ("ID_User");


--
-- TOC entry 2213 (class 2606 OID 18080)
-- Name: Vehicle_List vehicle_list_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY ("ID_Vehicle");


--
-- TOC entry 2215 (class 2606 OID 18082)
-- Name: Warnings warnings_pkey; Type: CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT warnings_pkey PRIMARY KEY ("ID_Warning");


--
-- TOC entry 2222 (class 2620 OID 18083)
-- Name: Leaderboard Add_User_Settings; Type: TRIGGER; Schema: livebot; Owner: livebot
--

CREATE TRIGGER "Add_User_Settings" AFTER INSERT ON livebot."Leaderboard" FOR EACH ROW EXECUTE PROCEDURE livebot."AddUserSettings"();


--
-- TOC entry 2217 (class 2606 OID 18084)
-- Name: User_Images background to user list; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT "background to user list" FOREIGN KEY ("BG_ID") REFERENCES livebot."Background_Image"("ID_BG");


--
-- TOC entry 2216 (class 2606 OID 18089)
-- Name: Server_Ranks server_ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Server_Ranks"
    ADD CONSTRAINT server_ranks_user_id_fkey FOREIGN KEY ("User_ID") REFERENCES livebot."Leaderboard"("ID_User");


--
-- TOC entry 2219 (class 2606 OID 18094)
-- Name: User_Settings user settings to user; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Settings"
    ADD CONSTRAINT "user settings to user" FOREIGN KEY ("User_ID") REFERENCES livebot."Leaderboard"("ID_User");


--
-- TOC entry 2218 (class 2606 OID 18099)
-- Name: User_Images user to leaderboard; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."User_Images"
    ADD CONSTRAINT "user to leaderboard" FOREIGN KEY ("User_ID") REFERENCES livebot."Leaderboard"("ID_User");


--
-- TOC entry 2220 (class 2606 OID 18104)
-- Name: Vehicle_List vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Vehicle_List"
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY ("Discipline") REFERENCES livebot."Discipline_List"("ID_Discipline");


--
-- TOC entry 2221 (class 2606 OID 18109)
-- Name: Warnings warnings_user_id_fkey; Type: FK CONSTRAINT; Schema: livebot; Owner: livebot
--

ALTER TABLE ONLY livebot."Warnings"
    ADD CONSTRAINT warnings_user_id_fkey FOREIGN KEY ("User_ID") REFERENCES livebot."User_Warnings"("ID_User");


-- Completed on 2019-06-10 03:30:48

--
-- PostgreSQL database dump complete
--

