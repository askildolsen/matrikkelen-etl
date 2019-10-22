DO
$$
DECLARE
    row record;
BEGIN
    FOR row IN SELECT schemaname FROM pg_tables WHERE tablename = 'matrikkeladressebruksenhet'
    LOOP
        EXECUTE 'ALTER SCHEMA ' || quote_ident(row.schemaname) || ' RENAME TO matrikkelenadresseleilighetsniva;';
    END LOOP;
END;
$$;
