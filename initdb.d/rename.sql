DO
$$
DECLARE
    row record;
BEGIN
    FOR row IN SELECT schemaname FROM pg_tables WHERE tablename = 'vegadressebruksenhet'
    LOOP
        EXECUTE 'ALTER SCHEMA ' || quote_ident(row.schemaname) || ' RENAME TO matrikkelen;';
    END LOOP;
END;
$$;
