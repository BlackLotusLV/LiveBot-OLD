--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.13
-- Dumped by pg_dump version 11.2

-- Started on 2019-06-08 16:52:06

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
-- TOC entry 3 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 2275 (class 0 OID 0)
-- Dependencies: 3
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 230 (class 1255 OID 16611)
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
-- TOC entry 217 (class 1255 OID 16605)
-- Name: add_new_user_picture(text); Type: FUNCTION; Schema: public; Owner: livebot
--

CREATE FUNCTION public.add_new_user_picture(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_images
	(user_id,bg_id)
	values (userid,1);$$;


ALTER FUNCTION public.add_new_user_picture(userid text) OWNER TO livebot;

--
-- TOC entry 216 (class 1255 OID 16604)
-- Name: add_new_user_settings(text); Type: FUNCTION; Schema: public; Owner: livebot
--

CREATE FUNCTION public.add_new_user_settings(userid text) RETURNS void
    LANGUAGE sql
    AS $$insert into user_settings
	(user_id, image_id, background_colour, text_colour, border_colour, user_info)
	values (userid,1,'white','black','black','Just a flesh wound');$$;


ALTER FUNCTION public.add_new_user_settings(userid text) OWNER TO livebot;

SET default_tablespace = '';

SET default_with_oids = false;

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
-- TOC entry 2276 (class 0 OID 0)
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
-- TOC entry 2277 (class 0 OID 0)
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
-- TOC entry 2278 (class 0 OID 0)
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
-- TOC entry 2279 (class 0 OID 0)
-- Dependencies: 192
-- Name: server_ranks_Id_server_rank_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public."server_ranks_Id_server_rank_seq" OWNED BY public.server_ranks.id_server_rank;


--
-- TOC entry 209 (class 1259 OID 17267)
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
-- TOC entry 208 (class 1259 OID 17265)
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
-- TOC entry 2280 (class 0 OID 0)
-- Dependencies: 208
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
-- TOC entry 2281 (class 0 OID 0)
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
-- TOC entry 2282 (class 0 OID 0)
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
-- TOC entry 2283 (class 0 OID 0)
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
    reason text COLLATE pg_catalog."C.UTF-8" NOT NULL,
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
-- TOC entry 2284 (class 0 OID 0)
-- Dependencies: 187
-- Name: warnings_id_warning_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: livebot
--

ALTER SEQUENCE public.warnings_id_warning_seq OWNED BY public.warnings.id_warning;


--
-- TOC entry 2116 (class 2604 OID 16541)
-- Name: background_image id_bg; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.background_image ALTER COLUMN id_bg SET DEFAULT nextval('public.backgrond_image_id_bg_seq'::regclass);


--
-- TOC entry 2119 (class 2604 OID 16620)
-- Name: discipline_list id_discipline; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list ALTER COLUMN id_discipline SET DEFAULT nextval('public.discipline_list_id_discipline_seq'::regclass);


--
-- TOC entry 2114 (class 2604 OID 16511)
-- Name: reaction_roles id; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.reaction_roles ALTER COLUMN id SET DEFAULT nextval('public.reaction_roles_id_seq'::regclass);


--
-- TOC entry 2115 (class 2604 OID 16525)
-- Name: server_ranks id_server_rank; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks ALTER COLUMN id_server_rank SET DEFAULT nextval('public."server_ranks_Id_server_rank_seq"'::regclass);


--
-- TOC entry 2121 (class 2604 OID 17270)
-- Name: stream_notification stream_notification_id; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.stream_notification ALTER COLUMN stream_notification_id SET DEFAULT nextval('public.stream_notification_stream_notification_id_seq'::regclass);


--
-- TOC entry 2117 (class 2604 OID 16558)
-- Name: user_images id_user_images; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images ALTER COLUMN id_user_images SET DEFAULT nextval('public.user_images_id_user_images_seq'::regclass);


--
-- TOC entry 2118 (class 2604 OID 16579)
-- Name: user_settings id_user_settings; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings ALTER COLUMN id_user_settings SET DEFAULT nextval('public.user_settings_id_user_settings_seq'::regclass);


--
-- TOC entry 2120 (class 2604 OID 16662)
-- Name: vehicle_list id_vehicle; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list ALTER COLUMN id_vehicle SET DEFAULT nextval('public.vehicle_list_id_vehicle_seq'::regclass);


--
-- TOC entry 2110 (class 2604 OID 16417)
-- Name: warnings id_warning; Type: DEFAULT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings ALTER COLUMN id_warning SET DEFAULT nextval('public.warnings_id_warning_seq'::regclass);


--
-- TOC entry 2133 (class 2606 OID 16546)
-- Name: background_image backgrond_image_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.background_image
    ADD CONSTRAINT backgrond_image_pkey PRIMARY KEY (id_bg);


--
-- TOC entry 2139 (class 2606 OID 16638)
-- Name: discipline_list discipline_list_discipline_name_key; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list
    ADD CONSTRAINT discipline_list_discipline_name_key UNIQUE (discipline_name);


--
-- TOC entry 2141 (class 2606 OID 16625)
-- Name: discipline_list discipline_list_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.discipline_list
    ADD CONSTRAINT discipline_list_pkey PRIMARY KEY (id_discipline);


--
-- TOC entry 2127 (class 2606 OID 16504)
-- Name: leaderboard leaderboard_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.leaderboard
    ADD CONSTRAINT leaderboard_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2129 (class 2606 OID 16516)
-- Name: reaction_roles reaction_roles_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.reaction_roles
    ADD CONSTRAINT reaction_roles_pkey PRIMARY KEY (id);


--
-- TOC entry 2131 (class 2606 OID 16530)
-- Name: server_ranks server_ranks_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks
    ADD CONSTRAINT server_ranks_pkey PRIMARY KEY (id_server_rank);


--
-- TOC entry 2145 (class 2606 OID 17275)
-- Name: stream_notification stream_notification_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.stream_notification
    ADD CONSTRAINT stream_notification_pkey PRIMARY KEY (stream_notification_id);


--
-- TOC entry 2135 (class 2606 OID 16563)
-- Name: user_images user_images_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT user_images_pkey PRIMARY KEY (id_user_images);


--
-- TOC entry 2137 (class 2606 OID 16584)
-- Name: user_settings user_settings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings
    ADD CONSTRAINT user_settings_pkey PRIMARY KEY (id_user_settings);


--
-- TOC entry 2123 (class 2606 OID 16448)
-- Name: user_warnings user_warnings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_warnings
    ADD CONSTRAINT user_warnings_pkey PRIMARY KEY (id_user);


--
-- TOC entry 2143 (class 2606 OID 16667)
-- Name: vehicle_list vehicle_list_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list
    ADD CONSTRAINT vehicle_list_pkey PRIMARY KEY (id_vehicle);


--
-- TOC entry 2125 (class 2606 OID 16422)
-- Name: warnings warnings_pkey; Type: CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings
    ADD CONSTRAINT warnings_pkey PRIMARY KEY (id_warning);


--
-- TOC entry 2152 (class 2620 OID 16613)
-- Name: leaderboard Add_User_Settings; Type: TRIGGER; Schema: public; Owner: livebot
--

CREATE TRIGGER "Add_User_Settings" AFTER INSERT ON public.leaderboard FOR EACH ROW EXECUTE PROCEDURE public."AddUserSettings"();


--
-- TOC entry 2149 (class 2606 OID 16569)
-- Name: user_images background to user list; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT "background to user list" FOREIGN KEY (bg_id) REFERENCES public.background_image(id_bg);


--
-- TOC entry 2147 (class 2606 OID 16531)
-- Name: server_ranks server_ranks_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.server_ranks
    ADD CONSTRAINT server_ranks_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2150 (class 2606 OID 16585)
-- Name: user_settings user settings to user; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_settings
    ADD CONSTRAINT "user settings to user" FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2148 (class 2606 OID 16564)
-- Name: user_images user to leaderboard; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.user_images
    ADD CONSTRAINT "user to leaderboard" FOREIGN KEY (user_id) REFERENCES public.leaderboard(id_user);


--
-- TOC entry 2151 (class 2606 OID 16668)
-- Name: vehicle_list vehicle_list_discipline_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.vehicle_list
    ADD CONSTRAINT vehicle_list_discipline_fkey FOREIGN KEY (discipline) REFERENCES public.discipline_list(id_discipline);


--
-- TOC entry 2146 (class 2606 OID 16449)
-- Name: warnings warnings_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: livebot
--

ALTER TABLE ONLY public.warnings
    ADD CONSTRAINT warnings_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.user_warnings(id_user);


-- Completed on 2019-06-08 16:52:16

--
-- PostgreSQL database dump complete
--

